using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.NWNX.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Perk.Archery
{
    public class RapidReload : IPerk
    {
        private readonly INWScript _;
        private readonly INWNXCreature _nwnxCreature;

        public RapidReload(INWScript script,
            INWNXCreature nwnxCreature)
        {
            _ = script;
            _nwnxCreature = nwnxCreature;
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
            _nwnxCreature.RemoveFeat(oPC, FEAT_RAPID_RELOAD);
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
            NWItem armor = oItem ?? oPC.Chest;
            if (armor.BaseItemType != BASE_ITEM_ARMOR) return;

            if (Equals(armor, oItem) || armor.CustomItemType != CustomItemType.LightArmor)
            {
                _nwnxCreature.RemoveFeat(oPC, FEAT_RAPID_RELOAD);
                return;
            }

            _nwnxCreature.AddFeat(oPC, FEAT_RAPID_RELOAD);
        }

        public bool IsHostile()
        {
            return false;
        }
    }
}
