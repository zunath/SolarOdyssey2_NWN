using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.NWNX.Contracts;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Perk.OneHanded
{
    public class BluntPowerAttack : IPerk
    {
        private readonly INWScript _;
        private readonly INWNXCreature _nwnxCreature;
        private readonly IPerkService _perk;

        public BluntPowerAttack(INWScript script,
            INWNXCreature nwnxCreature,
            IPerkService perk)
        {
            _ = script;
            _nwnxCreature = nwnxCreature;
            _perk = perk;
        }

        public bool CanCastSpell(NWPlayer oPC, NWObject oTarget)
        {
            return false;
        }

        public string CannotCastSpellMessage()
        {
            return null;
        }

        public int ManaCost(NWPlayer oPC, int baseManaCost)
        {
            return baseManaCost;
        }

        public float CastingTime(NWPlayer oPC, float baseCastingTime)
        {
            return baseCastingTime;
        }

        public float CooldownTime(NWPlayer oPC, float baseCooldownTime)
        {
            return baseCooldownTime;
        }

        public void OnImpact(NWPlayer oPC, NWObject oTarget)
        {
        }

        public void OnPurchased(NWPlayer oPC, int newLevel)
        {
            ApplyFeatChanges(oPC, null);
        }

        public void OnRemoved(NWPlayer oPC)
        {
            _nwnxCreature.RemoveFeat(oPC, FEAT_POWER_ATTACK);
            _nwnxCreature.RemoveFeat(oPC, FEAT_IMPROVED_POWER_ATTACK);
        }

        public void OnItemEquipped(NWPlayer oPC, NWItem oItem)
        {
            ApplyFeatChanges(oPC, null);
        }

        public void OnItemUnequipped(NWPlayer oPC, NWItem oItem)
        {
            ApplyFeatChanges(oPC, oItem);
        }

        private void ApplyFeatChanges(NWPlayer oPC, NWItem oItem)
        {
            NWItem equipped = oItem ?? oPC.RightHand;
            int perkLevel = _perk.GetPCPerkLevel(oPC, PerkType.BluntPowerAttack);

            if (Equals(equipped, oItem) || equipped.CustomItemType != CustomItemType.Blunt)
            {
                _nwnxCreature.RemoveFeat(oPC, FEAT_POWER_ATTACK);
                _nwnxCreature.RemoveFeat(oPC, FEAT_IMPROVED_POWER_ATTACK);
                return;
            }

            _nwnxCreature.AddFeat(oPC, FEAT_POWER_ATTACK);

            if (perkLevel >= 2)
            {
                _nwnxCreature.AddFeat(oPC, FEAT_IMPROVED_POWER_ATTACK);
            }
        }

        public bool IsHostile()
        {
            return false;
        }
    }
}
