using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.NWNX.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Perk.Throwing
{
    public class ZenTossing : IPerk
    {
        private readonly INWScript _;
        private readonly INWNXCreature _nwnxCreature;

        public ZenTossing(INWScript script,
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
            _nwnxCreature.RemoveFeat(oPC, FEAT_ZEN_ARCHERY);
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
            
            if (Equals(equipped, oItem) ||
                equipped.CustomItemType != CustomItemType.Throwing)
            {
                _nwnxCreature.RemoveFeat(oPC, FEAT_ZEN_ARCHERY);
                return;
            }

            _nwnxCreature.AddFeat(oPC, FEAT_ZEN_ARCHERY);
        }

        public bool IsHostile()
        {
            return false;
        }
    }
}
