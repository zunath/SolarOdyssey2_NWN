using System;
using NWN;
using SOO2.Game.Server.CustomEffect.Contracts;
using SOO2.Game.Server.GameObject;
using static NWN.NWScript;

namespace SOO2.Game.Server.CustomEffect
{
    public class PoisonEffect: ICustomEffect
    {
        private readonly INWScript _;

        public PoisonEffect(INWScript script)
        {
            _ = script;
        }

        public void Apply(NWCreature oCaster, NWObject oTarget)
        {
        }

        public void Tick(NWCreature oCaster, NWObject oTarget)
        {
            Random random = new Random();
            int amount = random.Next(3, 7);

            oCaster.AssignCommand(() =>
            {
                Effect damage = _.EffectDamage(amount);
                _.ApplyEffectToObject(DURATION_TYPE_INSTANT, damage, oTarget.Object);
            });
            
            Effect decreaseAC = _.EffectACDecrease(2);
            oCaster.AssignCommand(() =>
            {
                _.ApplyEffectToObject(DURATION_TYPE_TEMPORARY, decreaseAC, oTarget.Object, 6.1f);
            });
            
            _.ApplyEffectToObject(DURATION_TYPE_INSTANT, _.EffectVisualEffect(VFX_IMP_ACID_S), oTarget.Object);
        }

        public void WearOff(NWCreature oCaster, NWObject oTarget)
        {
        }
    }
}
