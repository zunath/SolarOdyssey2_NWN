using Freescape.Game.Server.CustomEffect.Contracts;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;

namespace Freescape.Game.Server.CustomEffect
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
