using NWN;
using SOO2.Game.Server.CustomEffect.Contracts;
using SOO2.Game.Server.GameObject;
using static NWN.NWScript;

namespace SOO2.Game.Server.CustomEffect
{
    public class ChainspellEffect: ICustomEffect
    {
        private readonly INWScript _;

        public ChainspellEffect(INWScript script)
        {
            _ = script;
        }

        public void Apply(NWCreature oCaster, NWObject oTarget)
        {
        }

        public void Tick(NWCreature oCaster, NWObject oTarget)
        {
            _.ApplyEffectToObject(DURATION_TYPE_TEMPORARY, _.EffectVisualEffect(VFX_IMP_EVIL_HELP), oTarget.Object, 6.1f);
        }

        public void WearOff(NWCreature oCaster, NWObject oTarget)
        {
        }
    }
}
