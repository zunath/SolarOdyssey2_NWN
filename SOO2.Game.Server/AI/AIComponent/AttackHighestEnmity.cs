using System.Linq;
using FluentBehaviourTree;
using NWN;
using SOO2.Game.Server.AI.Contracts;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.AI.AIComponent
{
    public class AttackHighestEnmity: IAIComponent
    {
        private readonly IEnmityService _enmity;
        private readonly INWScript _;

        public AttackHighestEnmity(IEnmityService enmity,
            INWScript script)
        {
            _enmity = enmity;
            _ = script;
        }

        public BehaviourTreeBuilder Build(BehaviourTreeBuilder builder, params object[] args)
        {
            NWCreature self = (NWCreature)args[0];

            return builder.Do("AttackHighestEnmity", t =>
            {
                var enmityTable = _enmity.GetEnmityTable(self);

                var target = enmityTable.Values
                    .OrderByDescending(o => o.TotalAmount)
                    .FirstOrDefault(x => x.TargetObject.IsValid &&
                                          x.TargetObject.Area.Equals(self.Area));
                
                self.AssignCommand(() =>
                {
                    if (target == null)
                    {
                        _.ClearAllActions();
                    }
                    else
                    {
                        _.ActionAttack(target.TargetObject.Object);
                    }
                });

                return BehaviourTreeStatus.Running;
            });
        }
    }
}
