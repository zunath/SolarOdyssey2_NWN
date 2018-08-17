using System.Linq;
using Freescape.Game.Server.Bioware.Contracts;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.NWNX.Contracts;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;
using Object = NWN.Object;

namespace Freescape.Game.Server.Placeable.CraftingForge
{
    public class OnDisturbed: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IPerkService _perk;
        private readonly ISkillService _skill;
        private readonly ICraftService _craft;
        private readonly IBiowarePosition _biowarePosition;
        private readonly INWNXPlayer _nwnxPlayer;

        public OnDisturbed(INWScript script,
            IPerkService perk,
            ISkillService skill,
            ICraftService craft,
            IBiowarePosition biowarePosition,
            INWNXPlayer nwnxPlayer)
        {
            _ = script;
            _perk = perk;
            _skill = skill;
            _craft = craft;
            _biowarePosition = biowarePosition;
            _nwnxPlayer = nwnxPlayer;
        }

        public bool Run(params object[] args)
        {
            if (_.GetInventoryDisturbType() != INVENTORY_DISTURB_TYPE_ADDED) return false;

            NWPlayer pc = NWPlayer.Wrap(_.GetLastDisturbed());
            NWItem item = NWItem.Wrap(_.GetInventoryDisturbItem());
            NWPlaceable forge = NWPlaceable.Wrap(Object.OBJECT_SELF);

            if (!checkValidity(forge, pc, item)) return false;
            startSmelt(forge, pc, item);
            return true;
        }


        private bool checkValidity(NWPlaceable forge, NWPlayer pc, NWItem item)
        {
            if (pc.IsBusy)
            {
                ReturnItemToPC(pc, item, "You are too busy.");
                return false;
            }

            if (_.GetIsObjectValid(forge.GetLocalObject("FORGE_USER")) == TRUE)
            {
                ReturnItemToPC(pc, item, "This forge is currently in use. Please wait...");
                return false;
            }

            string[] allowed = {
                "coal",
                "copper_ore",
                "tin_ore",
                "iron_ore",
                "gold_ore",
                "platinum_ore",
                "adamantium_ore",
                "cobalt_ore",
                "silver_ore",
                "titanium_ore",
                "mithril_ore"
        };

            if (!allowed.Contains(item.Resref))
            {
                ReturnItemToPC(pc, item, "Only coal and ore may be placed inside.");
                return false;
            }

            int level = _craft.GetIngotLevel(item.Resref);
            PCSkill pcSkill = _skill.GetPCSkill(pc, SkillType.Metalworking);
            if (pcSkill == null) return false;

            int delta = pcSkill.Rank - level;
            if (delta <= -4)
            {
                ReturnItemToPC(pc, item, "You do not have enough skill to smelt this ore.");
                return false;
            }

            int pcPerklevel = _perk.GetPCPerkLevel(pc, PerkType.Smelting);
            int orePerkLevel = _craft.GetIngotPerkLevel(item.Resref);

            if (pcPerklevel < orePerkLevel)
            {
                ReturnItemToPC(pc, item, "You do not have the perk necessary to smelt this ore.");
                return false;
            }

            return true;
        }

        private void startSmelt(NWPlaceable forge, NWPlayer pc, NWItem item)
        {
            int charges = forge.GetLocalInt("FORGE_CHARGES");
            if (item.Resref == "coal")
            {
                item.Destroy();
                charges += 10 + CalculatePerkCoalBonusCharges(pc);
                forge.SetLocalInt("FORGE_CHARGES", charges);

                NWPlaceable flames = NWPlaceable.Wrap(forge.GetLocalObject("FORGE_FLAMES"));
                if (!flames.IsValid)
                {
                    Vector flamePosition = _biowarePosition.GetChangedPosition(forge.Position, 0.36f, forge.Facing);
                    Location flameLocation = _.Location(forge.Area.Object, flamePosition, 0.0f);
                    flames = NWPlaceable.Wrap(_.CreateObject(OBJECT_TYPE_PLACEABLE, "forge_flame", flameLocation));
                    forge.SetLocalObject("FORGE_FLAMES", flames.Object);
                }

                return;
            }
            else if (charges <= 0)
            {
                ReturnItemToPC(pc, item, "You must light the forge with coal before smelting.");
                return;
            }
            item.Destroy();

            // Ready to smelt
            float baseCraftDelay = 18.0f - (_perk.GetPCPerkLevel(pc, PerkType.SpeedySmelter) * 0.1f);

            pc.IsBusy = true;
            forge.SetLocalObject("FORGE_USER", pc.Object);
            pc.SetLocalObject("FORGE", forge.Object);
            forge.SetLocalString("FORGE_ORE", item.Resref);

            _nwnxPlayer.StartGuiTimingBar(pc, baseCraftDelay, "cft_finish_smelt");

            _.ApplyEffectToObject(DURATION_TYPE_TEMPORARY, _.EffectCutsceneImmobilize(), pc.Object, baseCraftDelay);
            pc.AssignCommand(() => _.ActionPlayAnimation(ANIMATION_LOOPING_GET_MID, 1.0f, baseCraftDelay));
        }

        private void ReturnItemToPC(NWPlayer pc, NWItem item, string message)
        {
            _.CopyItem(item.Object, pc.Object, TRUE);
            item.Destroy();
            pc.SendMessage(message);
        }

        private int CalculatePerkCoalBonusCharges(NWPlayer pc)
        {
            int perkLevel = _perk.GetPCPerkLevel(pc, PerkType.CoalManagement);

            switch (perkLevel)
            {
                case 1: return 2;
                case 2: return 3;
                case 3: return 4;
                case 4: return 5;
                case 5: return 8;
                case 6: return 10;
                default: return 0;
            }
        }
    }
}
