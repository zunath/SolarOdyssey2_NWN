using System;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Perk.OneHanded
{
    public class SneakAttack: IPerk
    {
        public bool CanCastSpell(NWPlayer oPC, NWObject oTarget)
        {
            NWItem weapon = oPC.RightHand;
            NWItem armor = oPC.Chest;

            return weapon.CustomItemType == CustomItemType.FinesseBlade &&
                   armor.CustomItemType == CustomItemType.LightArmor;
        }

        public string CannotCastSpellMessage(NWPlayer oPC, NWObject oTarget)
        {
            return "You must be equipped with a finesse blade and light armor to use that ability.";
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
        }

        public void OnRemoved(NWPlayer oPC)
        {
        }

        public void OnItemEquipped(NWPlayer oPC, NWItem oItem)
        {
        }

        public void OnItemUnequipped(NWPlayer oPC, NWItem oItem)
        {
        }

        public void OnCustomEnmityRule(NWPlayer oPC, int amount)
        {
        }

        public bool IsHostile()
        {
            return false;
        }
    }
}
