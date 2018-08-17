using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Freescape.Game.Server.Data.Contracts;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.NWNX.Contracts;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Service
{
    public class CraftService : ICraftService
    {
        private readonly INWScript _;
        private readonly IDataContext _db;
        private readonly IPerkService _perk;
        private readonly ISkillService _skill;
        private readonly IColorTokenService _color;
        private readonly IItemService _item;
        private readonly INWNXPlayer _nwnxPlayer;
        private readonly IFoodService _food;
        private readonly IRandomService _random;
        private readonly IDurabilityService _durability;
        private readonly IErrorService _error;

        public CraftService(
            INWScript script,
            IDataContext db,
            IPerkService perk,
            ISkillService skill,
            IColorTokenService color,
            IItemService item,
            INWNXPlayer nwnxPlayer,
            IFoodService food,
            IRandomService random,
            IDurabilityService durability,
            IErrorService error)
        {
            _ = script;
            _db = db;
            _perk = perk;
            _skill = skill;
            _color = color;
            _item = item;
            _nwnxPlayer = nwnxPlayer;
            _food = food;
            _random = random;
            _durability = durability;
            _error = error;
        }

        private const float BaseCraftDelay = 18.0f;
        
        public List<CraftBlueprintCategory> GetCategoriesAvailableToPCByDeviceID(string playerID, int deviceID)
        {
            return _db.StoredProcedure<CraftBlueprintCategory>("GetCraftCategoriesAvailableToPCByDeviceID",
                new SqlParameter("DeviceID", deviceID),
                new SqlParameter("PlayerID", playerID));
        }

        public List<CraftBlueprint> GetPCBlueprintsByDeviceAndCategoryID(string playerID, int deviceID, int categoryID)
        {
            return _db.StoredProcedure<CraftBlueprint>("GetPCCraftBlueprintsByDeviceAndCategoryID",
                new SqlParameter("DeviceID", deviceID),
                new SqlParameter("CraftCategoryID", categoryID),
                new SqlParameter("PlayerID", playerID));
        }

        public string BuildBlueprintHeader(NWPlayer player, int blueprintID)
        {
            CraftBlueprint blueprint = _db.CraftBlueprints.Single(x => x.CraftBlueprintID == blueprintID);
            PCSkill pcSkill = _db.PCSkills.Single(x => x.PlayerID == player.GlobalID && x.SkillID == blueprint.SkillID);
            
            string header = _color.Green("Blueprint: ") + _color.White(blueprint.ItemName) + "\n\n";
            header += _color.Green("Skill: ") + _color.White(pcSkill.Skill.Name) + "\n";

            if (blueprint.CraftTierLevel > 0)
            {
                header += _color.Green("Required Tool Level: ") + blueprint.CraftTierLevel + "\n";
            }

            header += _color.Green("Difficulty: ") + CalculateDifficulty(pcSkill.Rank, blueprint.Level) + "\n\n";
            header += _color.Green("Components: ") + "\n\n";

            foreach (CraftBlueprintComponent component in blueprint.CraftBlueprintComponents)
            {
                string name = _item.GetNameByResref(component.ItemResref);
                header += _color.White(component.Quantity + "x " + name) + "\n";
            }

            return header;
        }

        public CraftBlueprint GetBlueprintByID(int craftBlueprintID)
        {
            return _db.CraftBlueprints.SingleOrDefault(x => x.CraftBlueprintID == craftBlueprintID);
        }

        public CraftBlueprint GetBlueprintKnownByPC(string playerID, int blueprintID, int deviceID)
        {
            return _db.StoredProcedureSingle<CraftBlueprint>("GetBlueprintKnownByPC",
                new SqlParameter("PlayerID", playerID),
                new SqlParameter("BlueprintID", blueprintID),
                new SqlParameter("DeviceID", deviceID));
        }

        public List<CraftBlueprintCategory> GetCategoriesAvailableToPC(string playerID)
        {
            return _db.StoredProcedure<CraftBlueprintCategory>("GetCategoriesAvailableToPC",
                new SqlParameter("PlayerID", playerID));
        }

        public List<CraftBlueprint> GetPCBlueprintsByCategoryID(string playerID, int categoryID)
        {
            return _db.StoredProcedure<CraftBlueprint>("GetPCBlueprintsByCategoryID",
                new SqlParameter("PlayerID", playerID),
                new SqlParameter("CraftCategoryID", categoryID));
        }


        public void CraftItem(NWPlayer oPC, NWPlaceable device, int blueprintID)
        {
            CraftBlueprint blueprint = _db.CraftBlueprints.Single(x => x.CraftBlueprintID == blueprintID);
            if (blueprint == null) return;
            bool requiresTools = false;
            bool foundTools = false;

            if (oPC.IsBusy)
            {
                oPC.SendMessage("You are too busy right now.");
                return;
            }

            // Check for tools, if necessary.
            if (blueprint.CraftTierLevel > 0)
            {
                requiresTools = true;
                NWItem tools = NWItem.Wrap(device.GetLocalObject("CRAFT_DEVICE_TOOLS"));
                if (tools.IsValid)
                {
                    foundTools = true;
                }
            }

            if (requiresTools != foundTools)
            {
                oPC.SendMessage(_color.Red("Tools were not found. Please place the tools you wish to use inside the crafting device."));
                oPC.IsBusy = false;
                return;
            }
            oPC.IsBusy = true;

            bool allComponentsFound = CheckItemCounts(oPC, device, blueprint.CraftBlueprintComponents);

            if (allComponentsFound)
            {
                float modifiedCraftDelay = CalculateCraftingDelay(oPC, blueprint.SkillID);
                
                _.ApplyEffectToObject(DURATION_TYPE_TEMPORARY, _.EffectCutsceneImmobilize(), oPC.Object, modifiedCraftDelay + 0.1f);
                oPC.AssignCommand(() =>
                {
                    _.ClearAllActions();
                    _.ActionPlayAnimation(ANIMATION_LOOPING_GET_MID, 1.0f, modifiedCraftDelay);
                });
                device.DelayCommand(() =>
                {
                    _.ApplyEffectToObject(DURATION_TYPE_INSTANT, _.EffectVisualEffect(VFX_COM_BLOOD_SPARK_MEDIUM), device.Object);
                }, 1.0f * (modifiedCraftDelay / 2.0f));

                _nwnxPlayer.StartGuiTimingBar(oPC, modifiedCraftDelay, "");

                oPC.DelayCommand(() =>
                {
                    try
                    {
                        RunCreateItem(oPC, device, blueprintID);
                        oPC.IsBusy = false;
                    }
                    catch (Exception ex)
                    {
                        _error.LogError(ex);
                    }
                }, modifiedCraftDelay);
            }
            else
            {
                oPC.SendMessage(_color.Red("You are missing required components..."));
            }
        }


        private float CalculateCraftingDelay(NWPlayer oPC, int skillID)
        {
            PerkType perkType;
            float adjustedSpeed = 1.0f;
            SkillType skillType = (SkillType) skillID;

            if (skillType == SkillType.Metalworking) perkType = PerkType.SpeedyMetalworking;
            else if (skillType == SkillType.Weaponsmith) perkType = PerkType.SpeedyWeaponsmith;
            else if (skillType == SkillType.Armorsmith) perkType = PerkType.SpeedyArmorsmith;
            else if (skillType == SkillType.Cooking) perkType = PerkType.SpeedyCooking;
            else if (skillType == SkillType.Woodworking) perkType = PerkType.SpeedyWoodworking;
            else return BaseCraftDelay;

            int perkLevel = _perk.GetPCPerkLevel(oPC, perkType);

            switch (perkLevel)
            {
                case 1: adjustedSpeed = 0.9f; break;
                case 2: adjustedSpeed = 0.8f; break;
                case 3: adjustedSpeed = 0.7f; break;
                case 4: adjustedSpeed = 0.6f; break;
                case 5: adjustedSpeed = 0.5f; break;
                case 6: adjustedSpeed = 0.4f; break;
                case 7: adjustedSpeed = 0.3f; break;
                case 8: adjustedSpeed = 0.2f; break;
                case 9: adjustedSpeed = 0.1f; break;
                case 10: adjustedSpeed = 0.01f; break;
            }

            return BaseCraftDelay * adjustedSpeed;
        }

        private bool CheckItemCounts(NWPlayer oPC, NWPlaceable device, ICollection<CraftBlueprintComponent> componentList)
        {
            bool allComponentsFound = false;
            Dictionary<string, int> components = new Dictionary<string, int>();

            foreach (CraftBlueprintComponent component in componentList)
            {
                components.Add(component.ItemResref, component.Quantity);
            }

            NWPlaceable tempStorage = NWPlaceable.Wrap(_.CreateObject(OBJECT_TYPE_PLACEABLE, "craft_temp_store", device.Location));
            device.SetLocalObject("CRAFT_TEMP_STORAGE", tempStorage.Object);

            foreach (NWItem item in device.InventoryItems)
            {
                if (components.ContainsKey(item.Resref) && components[item.Resref] > 0)
                {
                    components[item.Resref] = components[item.Resref] - 1;

                    _.CopyItem(item.Object, tempStorage.Object, TRUE);
                    item.Destroy();
                }

                int remainingQuantities = 0;
                foreach (int quantity in components.Values)
                    remainingQuantities += quantity;

                if (remainingQuantities <= 0)
                {
                    allComponentsFound = true;
                    break;
                }
            }

            if (!allComponentsFound)
            {
                foreach (NWItem item in tempStorage.InventoryItems)
                {
                    _.CopyItem(item.Object, device.Object, TRUE);
                    item.Destroy();
                }
                oPC.IsBusy = false;
            }

            return allComponentsFound;
        }

        private void RunCreateItem(NWPlayer oPC, NWPlaceable device, int blueprintID)
        {
            NWPlaceable tempStorage = NWPlaceable.Wrap(device.GetLocalObject("CRAFT_TEMP_STORAGE"));
            NWItem tools = NWItem.Wrap(device.GetLocalObject("CRAFT_DEVICE_TOOLS"));

            CraftBlueprint blueprint = _db.CraftBlueprints.Single(x => x.CraftBlueprintID == blueprintID);
            PCSkill pcSkill = _db.PCSkills.Single(x => x.PlayerID == oPC.GlobalID && x.SkillID == blueprint.SkillID);

            int pcEffectiveLevel = CalculatePCEffectiveLevel(oPC, device, pcSkill.Rank);


            float chance = CalculateChanceToCreateItem(pcEffectiveLevel, blueprint.Level);
            float roll = _random.RandomFloat() * 100.0f;
            float xpModifier;

            if (roll <= chance)
            {
                // Success!
                foreach (NWItem item in tempStorage.InventoryItems)
                {
                    item.Destroy();
                }

                NWItem craftedItem = NWItem.Wrap(_.CreateItemOnObject(blueprint.ItemResref, oPC.Object, blueprint.Quantity));
                craftedItem.IsIdentified = true;

                // If item isn't stackable, loop through and create as many as necessary.
                if (craftedItem.StackSize < blueprint.Quantity)
                {
                    for (int x = 2; x <= blueprint.Quantity; x++)
                    {
                        craftedItem = NWItem.Wrap(_.CreateItemOnObject(blueprint.ItemResref, oPC.Object));
                        craftedItem.IsIdentified = true;
                    }
                }

                oPC.SendMessage("You created " + blueprint.Quantity + "x " + blueprint.ItemName + "!");
                xpModifier = 1.0f;
            }
            else
            {
                // Failure...
                int chanceToLoseItems = 20;

                foreach (NWItem item in tempStorage.InventoryItems)
                {
                    if (_random.Random(100) > chanceToLoseItems)
                    {
                        _.CopyItem(item.Object, device.Object, TRUE);
                    }
                    item.Destroy();
                }

                oPC.SendMessage("You failed to create that item...");
                xpModifier = 0.2f;
            }

            float xp = _skill.CalculateSkillAdjustedXP(250, blueprint.Level, pcSkill.Rank) * xpModifier;
            tempStorage.Destroy();
            _skill.GiveSkillXP(oPC, blueprint.SkillID, (int)xp);

            if (tools.IsValid)
            {
                float min = 0.05f;
                float max = 0.1f;
                float reduceDurability = min + _random.RandomFloat() * (max - min);
                _durability.RunItemDecay(oPC, tools, reduceDurability);
            }

            if (_random.Random(100) + 1 <= 3)
            {
                _food.DecreaseHungerLevel(oPC, 1);
            }
        }

        private string CalculateDifficulty(int pcLevel, int blueprintLevel)
        {
            int delta = pcLevel - blueprintLevel;
            string difficulty = "";

            if (delta <= -5)
            {
                difficulty = _color.Custom("Impossible", 255, 62, 150);
            }
            else if (delta >= 4)
            {
                difficulty = _color.Custom("Trivial", 102, 255, 102);
            }
            else
            {
                switch (delta)
                {
                    case -4:
                        difficulty = _color.Custom("Extremely Difficult", 220, 20, 60);
                        break;
                    case -3:
                        difficulty = _color.Custom("Very Difficult", 255, 69, 0);
                        break;
                    case -2:
                        difficulty = _color.Custom("Difficult", 255, 165, 0);
                        break;
                    case -1:
                        difficulty = _color.Custom("Challenging", 238, 238, 0);
                        break;
                    case 0:
                        difficulty = _color.Custom("Moderate", 255, 255, 255);
                        break;
                    case 1:
                        difficulty = _color.Custom("Easy", 65, 105, 225);
                        break;
                    case 2:
                        difficulty = _color.Custom("Very Easy", 113, 113, 198);
                        break;
                    case 3:
                        difficulty = _color.Custom("Extremely Easy", 153, 255, 255);
                        break;
                }
            }


            return difficulty;
        }


        private float CalculateChanceToCreateItem(int pcLevel, int blueprintLevel)
        {
            int delta = pcLevel - blueprintLevel;
            float percentage = 0.0f;

            if (delta <= -5)
            {
                percentage = 0.0f;
            }
            else if (delta >= 4)
            {
                percentage = 95.0f;
            }
            else
            {
                switch (delta)
                {
                    case -4:
                        percentage = 15.0f;
                        break;
                    case -3:
                        percentage = 25.0f;
                        break;
                    case -2:
                        percentage = 40.0f;
                        break;
                    case -1:
                        percentage = 60.0f;
                        break;
                    case 0:
                        percentage = 75.0f;
                        break;
                    case 1:
                        percentage = 82.5f;
                        break;
                    case 2:
                        percentage = 87.0f;
                        break;
                    case 3:
                        percentage = 90.0f;
                        break;
                }
            }


            return percentage;
        }

        private int CalculatePCEffectiveLevel(NWPlayer pcGO, NWPlaceable device, int skillRank)
        {
            int deviceID = device.GetLocalInt("CRAFT_DEVICE_ID");
            NWItem tools = NWItem.Wrap(device.GetLocalObject("CRAFT_DEVICE_TOOLS"));
            int effectiveLevel = skillRank;
            PlayerCharacter player = _db.PlayerCharacters.Single(x => x.PlayerID == pcGO.GlobalID);

            if (tools.IsValid)
            {
                int toolBonus = 0;

                switch ((CraftDeviceType)deviceID)
                {
                    case CraftDeviceType.ArmorsmithBench: toolBonus = tools.CraftBonusArmorsmith; break;
                    case CraftDeviceType.Cookpot: toolBonus = tools.CraftBonusCooking; break;
                    case CraftDeviceType.MetalworkingBench: toolBonus = tools.CraftBonusMetalworking; break;
                    case CraftDeviceType.WeaponsmithBench: toolBonus = tools.CraftBonusWeaponsmith; break;
                    case CraftDeviceType.WoorkworkingBench: toolBonus = tools.CraftBonusWoodworking; break;
                }

                effectiveLevel += toolBonus;
            }

            switch ((CraftDeviceType)deviceID)
            {
                case CraftDeviceType.ArmorsmithBench:
                    if (player.BackgroundID == (int)BackgroundType.Armorsmith)
                        effectiveLevel++;
                    break;
                case CraftDeviceType.Cookpot:
                    if (player.BackgroundID == (int)BackgroundType.Chef)
                        effectiveLevel++;
                    break;
                case CraftDeviceType.MetalworkingBench:
                    if (player.BackgroundID == (int)BackgroundType.Metalworker)
                        effectiveLevel++;
                    break;
                case CraftDeviceType.WeaponsmithBench:
                    if (player.BackgroundID == (int)BackgroundType.Weaponsmith)
                        effectiveLevel++;
                    break;
                case CraftDeviceType.WoorkworkingBench:
                    if (player.BackgroundID == (int)BackgroundType.Woodworker)
                        effectiveLevel++;
                    break;
            }

            return effectiveLevel;
        }


        public string GetIngotResref(string oreResref)
        {
            string ingotResref;
            switch (oreResref)
            {
                case "copper_ore":
                    ingotResref = "copper_ingot";
                    break;
                case "tin_ore":
                    ingotResref = "tin_ingot";
                    break;
                case "iron_ore":
                    ingotResref = "iron_ingot";
                    break;
                case "gold_ore":
                    ingotResref = "gold_ingot";
                    break;
                case "platinum_ore":
                    ingotResref = "platinum_ingot";
                    break;
                case "adamantium_ore":
                    ingotResref = "adamantium_ingot";
                    break;
                case "cobalt_ore":
                    ingotResref = "cobalt_ingot";
                    break;
                case "silver_ore":
                    ingotResref = "silver_ingot";
                    break;
                case "titanium_ore":
                    ingotResref = "titanium_ingot";
                    break;
                case "mithril_ore":
                    ingotResref = "mithril_ingot";
                    break;
                default:
                    return "";
            }

            return ingotResref;
        }

        public int GetIngotLevel(string oreResref)
        {
            int level;
            switch (oreResref)
            {
                case "copper_ore":
                    level = 3;
                    break;
                case "tin_ore":
                    level = 8;
                    break;
                case "iron_ore":
                    level = 13;
                    break;
                case "gold_ore":
                    level = 18;
                    break;
                case "platinum_ore":
                    level = 23;
                    break;
                case "adamantium_ore":
                    level = 28;
                    break;
                case "cobalt_ore":
                    level = 33;
                    break;
                case "silver_ore":
                    level = 38;
                    break;
                case "titanium_ore":
                    level = 43;
                    break;
                case "mithril_ore":
                    level = 48;
                    break;
                default:
                    return -1;
            }

            return level;
        }

        public int GetIngotPerkLevel(string oreResref)
        {
            int level;
            switch (oreResref)
            {
                case "copper_ore":
                case "coal":
                    level = 1;
                    break;
                case "tin_ore":
                    level = 2;
                    break;
                case "iron_ore":
                    level = 3;
                    break;
                case "gold_ore":
                    level = 4;
                    break;
                case "platinum_ore":
                    level = 5;
                    break;
                case "adamantium_ore":
                    level = 6;
                    break;
                case "cobalt_ore":
                    level = 7;
                    break;
                case "silver_ore":
                    level = 8;
                    break;
                case "titanium_ore":
                    level = 9;
                    break;
                case "mithril_ore":
                    level = 10;
                    break;
                default:
                    return -1;
            }

            return level;
        }
    }
}
