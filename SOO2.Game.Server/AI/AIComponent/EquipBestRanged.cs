using FluentBehaviourTree;
using NWN;
using SOO2.Game.Server.AI.Contracts;
using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.AI.AIComponent
{
    public class EquipBestRanged : IAIComponent
    {
        private readonly INWScript _;

        public EquipBestRanged(INWScript script)
        {
            _ = script;
        }

        public BehaviourTreeBuilder Build(BehaviourTreeBuilder builder, params object[] args)
        {
            return builder.Do("EquipBestRanged", t =>
            {
                NWCreature self = (NWCreature)args[0];
                if (!self.IsInCombat ||
                    !self.RightHand.IsRanged)
                    return BehaviourTreeStatus.Failure;

                self.AssignCommand(() =>
                {
                    _.ActionEquipMostDamagingRanged(new Object());
                });

                return BehaviourTreeStatus.Running;
            });
        }
    }
}
