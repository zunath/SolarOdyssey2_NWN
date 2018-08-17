using SOO2.Game.Server.CustomEffect.Contracts;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.CustomEffect
{
    public class AegisEffect: ICustomEffect
    {
        private readonly ISkillService _skill;

        public AegisEffect(ISkillService skill)
        {
            _skill = skill;
        }

        public void Apply(NWCreature oCaster, NWObject oTarget)
        {
            _skill.ApplyStatChanges((NWPlayer)oTarget, null);
        }

        public void Tick(NWCreature oCaster, NWObject oTarget)
        {
        }

        public void WearOff(NWCreature oCaster, NWObject oTarget)
        {
            _skill.ApplyStatChanges((NWPlayer)oTarget, null);
        }
    }
}
