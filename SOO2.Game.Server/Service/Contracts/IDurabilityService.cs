﻿using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface IDurabilityService
    {
        float GetDurability(NWItem item);
        float GetMaxDurability(NWItem item);
        bool IsValidDurabilityType(NWItem item);
        void OnHitCastSpell(NWPlayer oTarget);
        void OnModuleEquip();
        string OnModuleExamine(string existingDescription, NWObject examinedObject);
        void RunItemDecay(NWPlayer oPC, NWItem oItem);
        void RunItemDecay(NWPlayer oPC, NWItem oItem, float reduceAmount);
        void RunItemRepair(NWPlayer oPC, NWItem oItem, float amount);
        void SetDurability(NWItem item, float value);
        void SetMaxDurability(NWItem item, float value);
    }
}