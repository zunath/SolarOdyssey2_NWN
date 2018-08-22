using FluentBehaviourTree;
using NWN;
using SOO2.Game.Server.BehaviourComponent.Contracts;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using static NWN.NWScript;

namespace SOO2.Game.Server.BehaviourComponent
{
    public class RandomWalk: IBehaviourComponent
    {
        private readonly INWScript _;
        private readonly IEnmityService _enmity;
        private readonly IRandomService _random;

        public RandomWalk(INWScript script,
            IEnmityService enmity,
            IRandomService random)
        {
            _ = script;
            _enmity = enmity;
            _random = random;
        }

        public BehaviourTreeBuilder Build(BehaviourTreeBuilder builder, params object[] args)
        {
            NWCreature self = (NWCreature) args[0];

            return builder.Do("RandomWalk", t =>
            {
                if (self.IsInCombat || !_enmity.IsEnmityTableEmpty(self))
                {
                    if (_.GetCurrentAction(self.Object) == ACTION_RANDOMWALK)
                    {
                        self.ClearAllActions();
                    }

                    return BehaviourTreeStatus.Failure;
                }
                
                if (_.GetCurrentAction(self.Object) == ACTION_INVALID &&
                    _.IsInConversation(self.Object) == FALSE &&
                    _.GetCurrentAction(self.Object) != ACTION_RANDOMWALK &&
                    _random.Random(100) <= 25)
                {
                    self.AssignCommand(() => _.ActionRandomWalk());
                }
                return BehaviourTreeStatus.Running;
            });
        }
    }
}
