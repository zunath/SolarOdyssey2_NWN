using System;
using System.Linq;
using Freescape.Game.Server.Bioware.Contracts;
using Freescape.Game.Server.Data.Contracts;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Item.Contracts;
using Freescape.Game.Server.NWNX.Contracts;
using Freescape.Game.Server.Service.Contracts;
using Freescape.Game.Server.ValueObject;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Service
{
    public class ItemService : IItemService
    {
        private readonly INWScript _;
        private readonly IDataContext _db;
        private readonly IDurabilityService _durability;
        private readonly IBiowareXP2 _xp2;
        private readonly ISkillService _skill;
        private readonly IColorTokenService _color;
        private readonly INWNXItem _nwnxItem;
        private readonly INWNXPlayer _nwnxPlayer;

        public ItemService(
            INWScript script,
            IDataContext db,
            IDurabilityService durability,
            IBiowareXP2 xp2,
            ISkillService skill,
            IColorTokenService color,
            INWNXItem nwnxItem,
            INWNXPlayer nwnxPlayer)
        {
            _ = script;
            _db = db;
            _durability = durability;
            _xp2 = xp2;
            _skill = skill;
            _color = color;
            _nwnxItem = nwnxItem;
            _nwnxPlayer = nwnxPlayer;
        }

        public string GetNameByResref(string resref)
        {
            NWPlaceable tempStorage = NWPlaceable.Wrap(_.GetObjectByTag("TEMP_ITEM_STORAGE"));
            if (!tempStorage.IsValid)
            {
                Console.WriteLine("Could not locate temp item storage object. Create a placeable container in a non-accessible area with the tag TEMP_ITEM_STORAGE.");
                return null;
            }
            NWItem item = NWItem.Wrap(_.CreateItemOnObject(resref, tempStorage.Object));
            string name = item.Name;
            item.Destroy();
            return name;
        }

        public void OnModuleItemAcquired()
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetModuleItemAcquiredBy());

            if (!oPC.IsPlayer) return;

            NWItem item = NWItem.Wrap(_.GetModuleItemAcquired());
            ApplyItemFeatures(item);
        }

        public void OnModuleActivatedItem()
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetItemActivator());
            NWItem oItem = NWItem.Wrap(_.GetItemActivated());
            NWObject oTarget = NWObject.Wrap(_.GetItemActivatedTarget());
            Location targetLocation = _.GetItemActivatedTargetLocation();

            string className = oItem.GetLocalString("JAVA_SCRIPT");
            if (className == string.Empty) className = oItem.GetLocalString("ACTIVATE_JAVA_SCRIPT");
            if (className == string.Empty) className = oItem.GetLocalString("JAVA_ACTION_SCRIPT");
            if (className == string.Empty) className = oItem.GetLocalString("SCRIPT");
            if (className == string.Empty) return;

            oPC.ClearAllActions();

            // Remove "Item." prefix if it exists.
            if (className.StartsWith("Item."))
                className = className.Substring(5);

            IActionItem item = App.ResolveByInterface<IActionItem>("Item." + className);

            if (oPC.IsBusy)
            {
                oPC.SendMessage("You are busy.");
                return;
            }

            string invalidTargetMessage = item.IsValidTarget(oPC, oItem, oTarget, targetLocation);
            if (!string.IsNullOrWhiteSpace(invalidTargetMessage))
            {
                oPC.SendMessage(invalidTargetMessage);
                return;
            }

            if (item.MaxDistance() > 0.0f)
            {
                if (_.GetDistanceBetween(oPC.Object, oTarget.Object) > item.MaxDistance() ||
                    oPC.Area.Resref != oTarget.Area.Resref)
                {
                    oPC.SendMessage("Your target is too far away.");
                    return;
                }
            }

            CustomData customData = item.StartUseItem(oPC, oItem, oTarget, targetLocation);
            float delay = item.Seconds(oPC, oItem, oTarget, targetLocation, customData);
            int animationID = item.AnimationID();
            bool faceTarget = item.FaceTarget();
            Vector userPosition = oPC.Position;

            oPC.AssignCommand(() =>
            {
                oPC.IsBusy = true;
                if (faceTarget)
                    _.SetFacingPoint(oTarget.Position);
                if (animationID > 0)
                    _.ActionPlayAnimation(animationID, 1.0f, delay);
            });
            
            _nwnxPlayer.StartGuiTimingBar(oPC, delay, string.Empty);
            oPC.DelayCommand(() =>
            {
                FinishActionItem(item, oPC, oItem, oTarget, targetLocation, userPosition, customData);
            }, delay);
        }

        public void OnModuleHeartbeat()
        {
            NWModule module = NWModule.Get();

            foreach (NWPlayer pc in module.Players)
            {
                NWItem offHand = pc.LeftHand;
                if (offHand.BaseItemType == BASE_ITEM_TORCH)
                {
                    int charges = offHand.ReduceCharges();
                    if (charges <= 0)
                    {
                        offHand.Destroy();
                        pc.SendMessage("Your torch has burned out.");
                    }
                }
            }
        }

        public string OnModuleExamine(string existingDescription, NWPlayer examiner, NWObject examinedObject)
        {
            if (!examiner.IsPlayer) return existingDescription;
            if (examinedObject.ObjectType != OBJECT_TYPE_ITEM) return existingDescription;

            NWItem examinedItem = NWItem.Wrap(examinedObject.Object);
            ApplyItemFeatures(examinedItem);
            string description = "";

            if (examinedItem.RecommendedLevel > 0)
            {
                description += _color.Orange("Recommended Level: ") + examinedItem.RecommendedLevel + "\n";
            }
            if (examinedItem.AssociatedSkillID > 0)
            {
                PCSkill pcSkill = _skill.GetPCSkillByID(examiner.GlobalID, examinedItem.AssociatedSkillID);
                description += _color.Orange("Associated Skill: ") + pcSkill.Skill.Name + "\n";
            }
            if (examinedItem.CustomAC > 0)
            {
                description += _color.Orange("AC: ") + examinedItem.CustomAC + "\n";
            }
            if (examinedItem.HPBonus > 0)
            {
                description += _color.Orange("HP Bonus: ") + examinedItem.HPBonus + "\n";
            }
            if (examinedItem.ManaBonus > 0)
            {
                description += _color.Orange("Mana Bonus: ") + examinedItem.ManaBonus + "\n";
            }
            if (examinedItem.CastingSpeed > 0)
            {
                description += _color.Orange("Casting Speed: +") + examinedItem.CastingSpeed + "%\n";
            }
            else if (examinedItem.CastingSpeed < 0)
            {
                description += _color.Orange("Casting Penalty: -") + examinedItem.CastingSpeed + "%\n";
            }

            if (examinedItem.LoggingBonus > 0)
            {
                description += _color.Orange("Logging Bonus: ") + examinedItem.LoggingBonus + "\n";
            }
            if (examinedItem.MiningBonus > 0)
            {
                description += _color.Orange("Mining Bonus: ") + examinedItem.MiningBonus + "\n";
            }
            if (examinedItem.CraftBonusMetalworking > 0)
            {
                description += _color.Orange("Metalworking Bonus: ") + examinedItem.CraftBonusMetalworking + "\n";
            }
            if (examinedItem.CraftBonusArmorsmith > 0)
            {
                description += _color.Orange("Armorsmith Bonus: ") + examinedItem.CraftBonusArmorsmith + "\n";
            }
            if (examinedItem.CraftBonusWeaponsmith > 0)
            {
                description += _color.Orange("Weaponsmith Bonus: ") + examinedItem.CraftBonusWeaponsmith + "\n";
            }
            if (examinedItem.CraftBonusCooking > 0)
            {
                description += _color.Orange("Cooking Bonus: ") + examinedItem.CraftBonusCooking + "\n";
            }
            if (examinedItem.CraftTierLevel > 0)
            {
                description += _color.Orange("Tool Level: ") + examinedItem.CraftTierLevel + "\n";
            }

            return existingDescription + "\n" + description;
        }

        public void OnModuleEquipItem()
        {
            int[] validItemTypes = {
                BASE_ITEM_ARMOR,
                BASE_ITEM_ARROW,
                BASE_ITEM_BASTARDSWORD,
                BASE_ITEM_BATTLEAXE,
                BASE_ITEM_BELT,
                BASE_ITEM_BOLT,
                BASE_ITEM_BOOTS,
                BASE_ITEM_BRACER,
                BASE_ITEM_BULLET,
                BASE_ITEM_CLOAK,
                BASE_ITEM_CLUB,
                BASE_ITEM_DAGGER,
                BASE_ITEM_DART,
                BASE_ITEM_DIREMACE,
                BASE_ITEM_DOUBLEAXE,
                BASE_ITEM_DWARVENWARAXE,
                BASE_ITEM_GLOVES,
                BASE_ITEM_GREATAXE,
                BASE_ITEM_GREATSWORD,
                BASE_ITEM_GRENADE,
                BASE_ITEM_HALBERD,
                BASE_ITEM_HANDAXE,
                BASE_ITEM_HEAVYCROSSBOW,
                BASE_ITEM_HEAVYFLAIL,
                BASE_ITEM_HELMET,
                BASE_ITEM_KAMA,
                BASE_ITEM_KATANA,
                BASE_ITEM_KUKRI,
                BASE_ITEM_LARGESHIELD,
                BASE_ITEM_LIGHTCROSSBOW,
                BASE_ITEM_LIGHTFLAIL,
                BASE_ITEM_LIGHTHAMMER,
                BASE_ITEM_LIGHTMACE,
                BASE_ITEM_LONGBOW,
                BASE_ITEM_LONGSWORD,
                BASE_ITEM_MORNINGSTAR,
                BASE_ITEM_QUARTERSTAFF,
                BASE_ITEM_RAPIER,
                BASE_ITEM_SCIMITAR,
                BASE_ITEM_SCYTHE,
                BASE_ITEM_SHORTBOW,
                BASE_ITEM_SHORTSPEAR,
                BASE_ITEM_SHORTSWORD,
                BASE_ITEM_SHURIKEN,
                BASE_ITEM_SICKLE,
                BASE_ITEM_SLING,
                BASE_ITEM_SMALLSHIELD,
                BASE_ITEM_THROWINGAXE,
                BASE_ITEM_TOWERSHIELD,
                BASE_ITEM_TRIDENT,
                BASE_ITEM_TWOBLADEDSWORD,
                BASE_ITEM_WARHAMMER,
                BASE_ITEM_WHIP
        };

            NWItem oItem = NWItem.Wrap(_.GetPCItemLastEquipped());
            int baseItemType = oItem.BaseItemType;

            if (!validItemTypes.Contains(baseItemType)) return;

            foreach (ItemProperty ip in oItem.ItemProperties)
            {
                if (_.GetItemPropertyType(ip) == ITEM_PROPERTY_ONHITCASTSPELL)
                {
                    if (_.GetItemPropertySubType(ip) == IP_CONST_ONHIT_CASTSPELL_ONHIT_UNIQUEPOWER)
                    {
                        return;
                    }
                }
            }

            // No item property found. Add it to the item.
            _xp2.IPSafeAddItemProperty(oItem, _.ItemPropertyOnHitCastSpell(IP_CONST_ONHIT_CASTSPELL_ONHIT_UNIQUEPOWER, 40), 0.0f, AddItemPropertyPolicy.ReplaceExisting, false, false);

            if (baseItemType == BASE_ITEM_TORCH)
            {
                int charges = oItem.ReduceCharges();
                if (charges <= 0)
                {
                    oItem.Destroy();
                }
            }
        }

        public void ReturnItem(NWObject target, NWItem item)
        {
            _.CopyItem(item.Object, target.Object, TRUE);
            item.Destroy();
        }

        public void StripAllItemProperties(NWItem item)
        {
            foreach (var ip in item.ItemProperties)
            {
                _.RemoveItemProperty(item.Object, ip);
            }
        }
        
        private void FinishActionItem(IActionItem actionItem, NWPlayer user, NWItem item, NWObject target, Location targetLocation, Vector userStartPosition, CustomData customData)
        {
            user.IsBusy = false;

            Vector userPosition = user.Position;
            if (userPosition.m_X != userStartPosition.m_X ||
                userPosition.m_Y != userStartPosition.m_Y ||
                userPosition.m_Z != userStartPosition.m_Z)
            {
                user.SendMessage("You move and interrupt your action.");
                return;
            }

            if (actionItem.MaxDistance() > 0.0f)
            {
                if (_.GetDistanceBetween(user.Object, target.Object) > actionItem.MaxDistance() ||
                    user.Area.Resref != target.Area.Resref)
                {
                    user.SendMessage("Your target is too far away.");
                    return;
                }
            }

            if (!target.IsValid && !actionItem.AllowLocationTarget())
            {
                user.SendMessage("Unable to locate target.");
                return;
            }

            string invalidTargetMessage = actionItem.IsValidTarget(user, item, target, targetLocation);
            if (!string.IsNullOrWhiteSpace(invalidTargetMessage))
            {
                user.SendMessage(invalidTargetMessage);
                return;
            }

            actionItem.ApplyEffects(user, item, target, targetLocation, customData);

            if (actionItem.ReducesItemCharge(user, item, target, targetLocation, customData))
            {
                if (item.Charges > 0) item.ReduceCharges();
                else item.Destroy();
            }
        }

        private Data.Entities.Item GetItemEntity(NWItem item)
        {
            return _db.Items.SingleOrDefault(x => x.Resref == item.Resref);
        }

        private void ApplyItemFeatures(NWItem item)
        {
            Data.Entities.Item entity = GetItemEntity(item);

            if (entity == null) return;

            item.CustomAC = entity.AC;
            item.CustomItemType = (CustomItemType) entity.ItemTypeID;
            item.RecommendedLevel = entity.RecommendedLevel;
            item.LoggingBonus = entity.LoggingBonus;
            item.MiningBonus = entity.MiningBonus;
            item.CastingSpeed = entity.CastingSpeed;
            item.CraftBonusMetalworking = entity.CraftBonusMetalworking;
            item.CraftBonusArmorsmith = entity.CraftBonusArmorsmith;
            item.CraftBonusWeaponsmith = entity.CraftBonusWeaponsmith;
            item.CraftBonusCooking = entity.CraftBonusCooking;
            item.CraftBonusWoodworking = entity.CraftBonusWoodworking;
            item.AssociatedSkillID = entity.AssociatedSkillID;
            item.CraftTierLevel = entity.CraftTierLevel;
            item.HPBonus = entity.HPBonus;
            item.ManaBonus = entity.ManaBonus;

            if (entity.Weight > 0)
            {
                _nwnxItem.SetWeight(item, entity.Weight);
            }

            if (entity.DurabilityPoints > 0)
            {
                _durability.SetMaxDurability(item, entity.DurabilityPoints);
            }
        }
    }
}
