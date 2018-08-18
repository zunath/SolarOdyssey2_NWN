using System;
using System.Linq;
using FluentBehaviourTree;
using NWN;
using SOO2.Game.Server.BehaviourComponent.Contracts;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using static NWN.NWScript;

namespace SOO2.Game.Server.BehaviourComponent
{
    public class AttackHighestEnmity: IBehaviourComponent
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
                if (_.GetIsInCombat(self.Object) == FALSE) return BehaviourTreeStatus.Failure;

                var enmityTable = _enmity.GetEnmityTable(self);

                var target = enmityTable.Values
                    .OrderByDescending(o => o.Amount)
                    .SingleOrDefault(x => x.TargetObject.IsValid &&
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
