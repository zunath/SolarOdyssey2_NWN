﻿using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Perk.TwoHanded
{
    public class Knockdown: IPerk
    {
        private readonly INWScript _;
        private readonly IPerkService _perk;
        private readonly IRandomService _random;

        public Knockdown(INWScript script,
            IPerkService perk,
            IRandomService random)
        {
            _ = script;
            _perk = perk;
            _random = random;
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
            int perkLevel = _perk.GetPCPerkLevel(oPC, PerkType.Knockdown);
            int chance;
            float length;

            switch (perkLevel)
            {
                case 1:
                    chance = 10;
                    length = 3.0f;
                    break;
                case 2:
                    chance = 20;
                    length = 3.0f;
                    break;
                case 3:
                    chance = 20;
                    length = 6.0f;
                    break;
                case 4:
                    chance = 30;
                    length = 6.0f;
                    break;
                case 5:
                    chance = 40;
                    length = 6.0f;
                    break;
                case 6:
                    chance = 50;
                    length = 6.0f;
                    break;
                default: return;
            }

            if (_random.Random(100) + 1 <= chance)
            {
                _.ApplyEffectToObject(DURATION_TYPE_TEMPORARY, _.EffectKnockdown(), oTarget.Object, length);
            }
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

        public bool IsHostile()
        {
            return false;
        }
    }
}
