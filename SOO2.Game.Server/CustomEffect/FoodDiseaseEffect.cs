using System;
using Freescape.Game.Server.CustomEffect.Contracts;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.CustomEffect
{
    public class FoodDiseaseEffect: ICustomEffect
    {
        private readonly INWScript _;
        private readonly ISkillService _skill;

        public FoodDiseaseEffect(INWScript script,
            ISkillService skill)
        {
            _ = script;
            _skill = skill;
        }

        public void Apply(NWCreature oCaster, NWObject oTarget)
        {
            Random random = new Random();

            Effect effect = _.EffectAbilityDecrease(ABILITY_STRENGTH, random.Next(1, 5));
            effect = _.EffectLinkEffects(effect, _.EffectAbilityDecrease(ABILITY_CONSTITUTION, random.Next(1, 5)));
            effect = _.EffectLinkEffects(effect, _.EffectAbilityDecrease(ABILITY_DEXTERITY,    random.Next(1, 5)));
            effect = _.EffectLinkEffects(effect, _.EffectAbilityDecrease(ABILITY_INTELLIGENCE, random.Next(1, 5)));
            effect = _.EffectLinkEffects(effect, _.EffectAbilityDecrease(ABILITY_WISDOM,       random.Next(1, 5)));
            effect = _.EffectLinkEffects(effect, _.EffectAbilityDecrease(ABILITY_CHARISMA,     random.Next(1, 5)));

            effect = _.TagEffect(effect, "FOOD_DISEASE_EFFECT");
            _.ApplyEffectToObject(DURATION_TYPE_PERMANENT, effect, oTarget.Object);

            _skill.ApplyStatChanges(NWPlayer.Wrap(oTarget.Object), null);
        }

        public void Tick(NWCreature oCaster, NWObject oTarget)
        {
        }

        public void WearOff(NWCreature oCaster, NWObject oTarget)
        {
            foreach (Effect effect in oTarget.Effects)
            {
                if (_.GetEffectTag(effect) == "FOOD_DISEASE_EFFECT")
                {
                    _.RemoveEffect(oTarget.Object, effect);
                }
            }

            oTarget.DelayCommand(() =>
            {
                _skill.ApplyStatChanges(NWPlayer.Wrap(oTarget.Object), null);
            }, 0.5f);

        }
    }
}
