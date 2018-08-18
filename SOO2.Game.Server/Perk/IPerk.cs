﻿using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Perk
{
    public interface IPerk
    {
        bool CanCastSpell(NWPlayer oPC, NWObject oTarget);
        string CannotCastSpellMessage();
        int ManaCost(NWPlayer oPC, int baseManaCost);
        float CastingTime(NWPlayer oPC, float baseCastingTime);
        float CooldownTime(NWPlayer oPC, float baseCooldownTime);
        void OnImpact(NWPlayer oPC, NWObject oTarget, int enmity);
        void OnPurchased(NWPlayer oPC, int newLevel);
        void OnRemoved(NWPlayer oPC);
        void OnItemEquipped(NWPlayer oPC, NWItem oItem);
        void OnItemUnequipped(NWPlayer oPC, NWItem oItem);
        bool IsHostile();
    }
}
