﻿using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.NWNX.Contracts;
using static NWN.NWScript;

namespace SOO2.Game.Server.Perk.Armor
{
    public class Dodge: IPerk
    {
        private readonly INWNXCreature _nwnxCreature;

        public Dodge(INWNXCreature nwnxCreature)
        {
            _nwnxCreature = nwnxCreature;
        }

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
            _nwnxCreature.RemoveFeat(oPC, FEAT_DODGE);
        }

        public void OnItemEquipped(NWPlayer oPC, NWItem oItem)
        {
            ApplyFeatChanges(oPC, null);
        }

        public void OnItemUnequipped(NWPlayer oPC, NWItem oItem)
        {
            ApplyFeatChanges(oPC, oItem);
        }

        public void OnCustomEnmityRule(NWPlayer oPC, int amount)
        {
        }

        private void ApplyFeatChanges(NWPlayer oPC, NWItem oItem)
        {
            NWItem equipped = oItem ?? oPC.Chest;
            
            if (equipped.Equals(oItem) || equipped.CustomItemType != CustomItemType.LightArmor)
            {
                _nwnxCreature.RemoveFeat(oPC, FEAT_DODGE);
                return;
            }

            _nwnxCreature.AddFeat(oPC, FEAT_DODGE);
        }

        public bool IsHostile()
        {
            return false;
        }
    }
}
