using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Perk.Alteration
{
    public class Purify: IPerk
    {
        private readonly INWScript _;
        private readonly IPerkService _perk;
        private readonly IRandomService _random;
        private readonly ICustomEffectService _customEffect;
        private readonly ISkillService _skill;

        public Purify(INWScript script,
            IPerkService perk,
            IRandomService random,
            ICustomEffectService customEffect,
            ISkillService skill)
        {
            _ = script;
            _perk = perk;
            _random = random;
            _customEffect = customEffect;
            _skill = skill;
        }

        public bool CanCastSpell(NWPlayer oPC, NWObject oTarget)
        {
            return oTarget.IsPlayer;
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
            int wisdom = oPC.WisdomModifier;
            int intelligence = oPC.IntelligenceModifier;
            float castingTime = baseCastingTime - ((wisdom * 1.5f) + (intelligence));

            if (castingTime < 1.0f) castingTime = 1.0f;

            return castingTime;
        }

        public float CooldownTime(NWPlayer oPC, float baseCooldownTime)
        {
            return baseCooldownTime;
        }

        public void OnImpact(NWPlayer oPC, NWObject oTarget)
        {
            int level = _perk.GetPCPerkLevel(oPC, PerkType.Purify);
            int luck = _perk.GetPCPerkLevel(oPC,  PerkType.Lucky);

            bool luckBonus = _random.Random(100) + 1 <= luck;

            if (level >= 1 || luckBonus)
            {
                _customEffect.RemovePCCustomEffect((NWPlayer)oTarget, CustomEffectType.Bleeding);
            }
            if (level >= 2 || luckBonus)
            {
                _customEffect.RemovePCCustomEffect((NWPlayer)oTarget, CustomEffectType.Poison);
            }
            if (level >= 3 || luckBonus)
            {
                _customEffect.RemovePCCustomEffect((NWPlayer)oTarget, CustomEffectType.Burning);
            }

            Effect vfx = _.EffectVisualEffect(VFX_IMP_HEALING_S);
            _.ApplyEffectToObject(DURATION_TYPE_INSTANT, vfx, oTarget.Object);

            foreach (Effect effect in oTarget.Effects)
            {
                int effectType = _.GetEffectType(effect);
                if (effectType == EFFECT_TYPE_POISON || effectType == EFFECT_TYPE_DISEASE)
                {
                    _.RemoveEffect(oTarget.Object, effect);
                }
            }

            _skill.RegisterPCToAllCombatTargetsForSkill(oPC, SkillType.AlterationMagic);
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
