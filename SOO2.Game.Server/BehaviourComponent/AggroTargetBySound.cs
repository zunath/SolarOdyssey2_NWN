using FluentBehaviourTree;
using NWN;
using SOO2.Game.Server.BehaviourComponent.Contracts;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using static NWN.NWScript;

namespace SOO2.Game.Server.BehaviourComponent
{
    public class AggroTargetBySound : IBehaviourComponent
    {
        private readonly INWScript _;
        private readonly IEnmityService _enmity;

        public AggroTargetBySound(INWScript script,
            IEnmityService enmity)
        {
            _ = script;
            _enmity = enmity;
        }

        public BehaviourTreeBuilder Build(BehaviourTreeBuilder builder, params object[] args)
        {
            return builder.Do("AggroTargetBySound", t =>
            {
                NWCreature self = (NWCreature)args[0];

                if (self.IsInCombat) return BehaviourTreeStatus.Failure;

                float aggroRange = self.GetLocalFloat("AGGRO_RANGE");
                if (aggroRange <= 0.0f) aggroRange = 5.0f;

                int nth = 1;
                NWCreature creature = NWCreature.Wrap(_.GetNearestObject(OBJECT_TYPE_CREATURE, self.Object, nth));
                while (creature.IsValid)
                {
                    if (_.GetIsEnemy(creature.Object, self.Object) == TRUE &&
                        !_enmity.IsOnEnmityTable(self, creature) &&
                        !creature.HasAnyEffect(EFFECT_TYPE_SANCTUARY) &&
                        _.GetDistanceBetween(self.Object, creature.Object) <= aggroRange && 
                        _.LineOfSightObject(self.Object, creature.Object) == TRUE)
                    {
                        _enmity.AdjustEnmity(self, creature, 0, 1);
                    }

                    nth++;
                    creature = NWCreature.Wrap(_.GetNearestObject(OBJECT_TYPE_CREATURE, self.Object, nth));
                }


                return BehaviourTreeStatus.Running;
            });
        }
    }
}
