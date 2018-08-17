using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.NWNX.Contracts;
using static NWN.NWScript;

namespace Freescape.Game.Server.Perk.Armor
{
    public class Mobility : IPerk
    {
        private readonly INWNXCreature _nwnxCreature;

        public Mobility(INWNXCreature nwnxCreature)
        {
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
            _nwnxCreature.RemoveFeat(oPC, FEAT_MOBILITY);
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
            NWItem equipped = oItem ?? oPC.Chest;

            if (equipped.Equals(oItem) || equipped.CustomItemType != CustomItemType.LightArmor)
            {
                _nwnxCreature.RemoveFeat(oPC, FEAT_MOBILITY);
                return;
            }

            _nwnxCreature.AddFeat(oPC, FEAT_MOBILITY);
        }

        public bool IsHostile()
        {
            return false;
        }
    }
}
