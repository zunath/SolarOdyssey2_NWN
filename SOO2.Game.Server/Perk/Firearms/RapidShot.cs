﻿using NWN;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.NWNX.Contracts;

namespace SOO2.Game.Server.Perk.Firearms
{
    public class RapidShot : IPerk
    {
        private readonly INWScript _;
        private readonly INWNXCreature _nwnxCreature;

        public RapidShot(INWScript script,
            INWNXCreature nwnxCreature)
        {
            _ = script;
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
            _nwnxCreature.RemoveFeat(oPC, NWScript.FEAT_RAPID_SHOT);
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
            NWItem equippedArmor = oItem ?? oPC.Chest;
            NWItem equippedWeapon = oItem ?? oPC.RightHand;

            if (equippedArmor.Equals(oItem) || equippedWeapon.Equals(oItem) || 
                equippedArmor.CustomItemType != CustomItemType.LightArmor ||
                equippedWeapon.CustomItemType != CustomItemType.Bow)
            {
                _nwnxCreature.RemoveFeat(oPC, NWScript.FEAT_RAPID_RELOAD);
                return;
            }

            _nwnxCreature.AddFeat(oPC, NWScript.FEAT_RAPID_RELOAD);
        }

        public bool IsHostile()
        {
            return false;
        }
    }
}
