using System;
using System.Linq;
using System.Xml.Linq;
using FluentBehaviourTree;
using SOO2.Game.Server.GameObject;
using NWN;
using SOO2.Game.Server.Service.Contracts;
using static NWN.NWScript;
using Object = NWN.Object;

namespace SOO2.Game.Server.Creature
{
    public class StandardCreature : CreatureBase
    {
        private readonly INWScript _;
        private readonly BehaviourTreeBuilder _builder;
        private readonly IEnmityService _enmity;

        public StandardCreature(BehaviourTreeBuilder builder,
            INWScript script,
            IEnmityService enmity)
        {
            _ = script;
            _builder = builder;
            _enmity = enmity;
        }

        public override bool IgnoreNWNEvents => true;

        public override IBehaviourTreeNode Behaviour
        {
            get
            {
                return _builder

                    .Parallel("Target highest enmity", 1, 2)
                    .Do("ClearInvalidEnmityTargets", t =>
                    {
                        var enmityList = _enmity.GetEnmityTable(Self)
                            .Where(x => x.Value.TargetObject == null ||
                                        !x.Value.TargetObject.Area.Equals(Self.Area) ||
                                        !x.Value.TargetObject.IsValid);
                        foreach (var obj in enmityList.ToArray())
                        {
                            Console.WriteLine("removing id = " + obj.Key);
                            _enmity.GetEnmityTable(Self).Remove(obj.Key);
                        }

                        return BehaviourTreeStatus.Running;
                    })
                    .Do("AttackHighestEnmity", t =>
                    {
                        if (_.GetIsInCombat(Self.Object) == FALSE) return BehaviourTreeStatus.Failure;

                        var enmityTable = _enmity.GetEnmityTable(Self);

                        var target = enmityTable.Values
                            .OrderByDescending(o => o.Amount)
                            .SingleOrDefault(x => x.TargetObject.IsValid &&
                                                  x.TargetObject.Area.Equals(Self.Area));

                        Self.AssignCommand(() =>
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
                    })
                    .End()
                    .Build();
            }
        }

        public override void OnPhysicalAttacked()
        {
            base.OnPhysicalAttacked();

            NWCreature attacker = NWCreature.Wrap(_.GetLastAttacker(Object.OBJECT_SELF));
            _enmity.AdjustEnmity(Self, attacker, 1);
        }

        public override void OnDamaged()
        {
            base.OnDamaged();

            NWCreature damager = NWCreature.Wrap(_.GetLastDamager(Object.OBJECT_SELF));
            int enmityAmount = _.GetTotalDamageDealt() / 3;
            if (enmityAmount <= 0) enmityAmount = 1;

            _enmity.AdjustEnmity(Self, damager, enmityAmount);
        }

    }
}
