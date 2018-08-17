﻿using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.NWNX.Contracts;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Perk.OneHanded
{
    public class DualWielding : IPerk
    {
        private readonly INWScript _;
        private readonly INWNXCreature _nwnxCreature;
        private readonly IPerkService _perk;

        public DualWielding(INWScript script,
            INWNXCreature nwnxCreature,
            IPerkService perk)
        {
            _ = script;
            _nwnxCreature = nwnxCreature;
            _perk = perk;
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
            RemoveFeats(oPC);
        }

        public void OnItemEquipped(NWPlayer oPC, NWItem oItem)
        {
            ApplyFeatChanges(oPC, null);
        }

        public void OnItemUnequipped(NWPlayer oPC, NWItem oItem)
        {
            ApplyFeatChanges(oPC, oItem);
        }


        private void RemoveFeats(NWPlayer oPC)
        {
            _nwnxCreature.RemoveFeat(oPC, FEAT_TWO_WEAPON_FIGHTING);
            _nwnxCreature.RemoveFeat(oPC, FEAT_AMBIDEXTERITY);
            _nwnxCreature.RemoveFeat(oPC, FEAT_IMPROVED_TWO_WEAPON_FIGHTING);
        }


        private void ApplyFeatChanges(NWPlayer oPC, NWItem oItem)
        {
            NWItem mainEquipped = oItem ?? oPC.RightHand;
            NWItem offEquipped = oItem ?? oPC.LeftHand;
            
            // oItem was unequipped.
            if (Equals(mainEquipped, oItem) || Equals(offEquipped, oItem))
            {
                RemoveFeats(oPC);
                return;
            }

            // Main or offhand was invalid (i.e not equipped)
            if (!mainEquipped.IsValid || !offEquipped.IsValid)
            {
                RemoveFeats(oPC);
                return;
            }

            // Main or offhand is not acceptable item type.
            if (mainEquipped.CustomItemType != CustomItemType.Blade ||
                mainEquipped.CustomItemType != CustomItemType.Blunt ||
                mainEquipped.CustomItemType != CustomItemType.FinesseBlade || 
                offEquipped.CustomItemType != CustomItemType.Blade || 
                offEquipped.CustomItemType != CustomItemType.Blunt || 
                offEquipped.CustomItemType != CustomItemType.FinesseBlade)
            {
                RemoveFeats(oPC);
                return;
            }


            int perkLevel = _perk.GetPCPerkLevel(oPC, PerkType.DualWielding);
            _nwnxCreature.AddFeat(oPC, FEAT_TWO_WEAPON_FIGHTING);

            if (perkLevel >= 2)
            {
                _nwnxCreature.AddFeat(oPC, FEAT_AMBIDEXTERITY);
            }
            if (perkLevel >= 3)
            {
                _nwnxCreature.AddFeat(oPC, FEAT_IMPROVED_TWO_WEAPON_FIGHTING);
            }
        }

        public bool IsHostile()
        {
            return false;
        }
    }
}
