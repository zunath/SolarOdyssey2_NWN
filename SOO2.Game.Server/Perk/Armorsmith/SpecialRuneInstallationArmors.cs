﻿using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Perk.Armorsmith
{
    public class SpecialRuneInstallationArmors : IPerk
    {
        public bool CanCastSpell(NWPlayer oPC, NWObject oTarget)
        {
            return false;
        }

        public string CannotCastSpellMessage(NWPlayer oPC, NWObject oTarget)
        {
            return null;
        }

        public int ManaCost(NWPlayer oPC, int baseManaCost)
        {
            return 0;
        }

        public float CastingTime(NWPlayer oPC, float baseCastingTime)
        {
            return 0;
        }

        public float CooldownTime(NWPlayer oPC, float baseCooldownTime)
        {
            return 0;
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
