using System;
using SOO2.Game.Server.CustomEffect.Contracts;
using SOO2.Game.Server.GameObject;
using NWN;
using static NWN.NWScript;

namespace SOO2.Game.Server.CustomEffect
{
    public class BurningEffect: ICustomEffect
    {
        private readonly INWScript _;

        public BurningEffect(INWScript script)
        {
            _ = script;
        }

        public void Apply(NWCreature oCaster, NWObject oTarget)
        {
        }

        public void Tick(NWCreature oCaster, NWObject oTarget)
        {
            Random random = new Random();
            int amount = random.Next(1, 2);
            oCaster.AssignCommand(() =>
            {
                Effect damage = _.EffectDamage(amount, DAMAGE_TYPE_FIRE);
                _.ApplyEffectToObject(DURATION_TYPE_INSTANT, damage, oTarget.Object);
            });

            Effect vfx = _.EffectVisualEffect(VFX_COM_HIT_FIRE);
            _.ApplyEffectToObject(DURATION_TYPE_INSTANT, vfx, oTarget.Object);
        }

        public void WearOff(NWCreature oCaster, NWObject oTarget)
        {
        }
    }
}
