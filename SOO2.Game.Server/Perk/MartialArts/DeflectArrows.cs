using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.NWNX.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Perk.MartialArts
{
    public class DeflectArrows : IPerk
    {
        private readonly INWScript _;
        private readonly INWNXCreature _nwnxCreature;

        public DeflectArrows(INWScript script,
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
            _nwnxCreature.RemoveFeat(oPC, FEAT_DEFLECT_ARROWS);
        }

        public void OnItemEquipped(NWPlayer oPC, NWItem oItem)
        {
            ApplyFeatChanges(oPC, null);
        }

        public void OnItemUnequipped(NWPlayer oPC, NWItem oItem)
        {
            ApplyFeatChanges(oPC, oItem);
        }

        private void ApplyFeatChanges(NWPlayer oPC, NWItem unequippingItem)
        {
            NWItem mainHand = oPC.RightHand;
            NWItem offHand = oPC.LeftHand;
            CustomItemType mainType = mainHand.CustomItemType;
            CustomItemType offType = offHand.CustomItemType;
            bool receivesFeat = true;

            if (unequippingItem != null && Equals(unequippingItem, mainHand))
            {
                mainHand = NWItem.Wrap(new Object());
            }
            else if (unequippingItem != null && Equals(unequippingItem, offHand))
            {
                offHand = NWItem.Wrap(new Object());
            }

            if ((!mainHand.IsValid && !offHand.IsValid) ||
                (mainType != CustomItemType.MartialArtWeapon || offType != CustomItemType.MartialArtWeapon))
            {
                receivesFeat = false;
            }

            if (receivesFeat)
            {
                _nwnxCreature.AddFeat(oPC, FEAT_DEFLECT_ARROWS);
            }
            else
            {
                _nwnxCreature.RemoveFeat(oPC, FEAT_DEFLECT_ARROWS);
            }
        }
        public bool IsHostile()
        {
            return false;
        }
    }
}
