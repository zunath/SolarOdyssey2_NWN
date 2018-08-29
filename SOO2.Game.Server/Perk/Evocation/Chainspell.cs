﻿using NWN;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using static NWN.NWScript;

namespace SOO2.Game.Server.Perk.Evocation
{
    public class Chainspell: IPerk
    {
        private readonly ICustomEffectService _customEffect;
        private readonly IPerkService _perk;
        private readonly INWScript _;

        public Chainspell(ICustomEffectService customEffect,
            IPerkService perk,
            INWScript script)
        {
            _customEffect = customEffect;
            _perk = perk;
            _ = script;
        }

        public bool CanCastSpell(NWPlayer oPC, NWObject oTarget)
        {
            return oPC.Chest.CustomItemType == CustomItemType.MysticArmor;
        }

        public string CannotCastSpellMessage(NWPlayer oPC, NWObject oTarget)
        {
            return "You must be equipped with mystic armor to activate that ability.";
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
            int perkLevel = _perk.GetPCPerkLevel(oPC, PerkType.Chainspell);
            int ticks = 10; // 60 seconds base

            if (perkLevel >= 2)
                ticks += 4; // +24 seconds (84 total)
            if (perkLevel >= 3)
                ticks += 4; // +24 seconds (108 total)

            _customEffect.ApplyCustomEffect(oPC, oPC, CustomEffectType.Chainspell, ticks, perkLevel);

            _.ApplyEffectToObject(DURATION_TYPE_TEMPORARY, _.EffectVisualEffect(VFX_IMP_EVIL_HELP), oPC.Object, 6.1f);
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
