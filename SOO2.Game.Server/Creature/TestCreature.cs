using System;
using FluentBehaviourTree;
using NWN;

namespace Freescape.Game.Server.Creature
{
    public class TestCreature: CreatureBase
    {
        private readonly INWScript _;
        private readonly BehaviourTreeBuilder _builder;

        public TestCreature(BehaviourTreeBuilder builder,
            INWScript script)
        {
            _ = script;
            _builder = builder;
        }

        public override bool IgnoreNWNEvents => false;
        
        public override IBehaviourTreeNode Behaviour
        {
            get
            {
                return _builder
                    .Sequence("seq1")
                    .Do("action1", t =>
                    {
                        var ticks = Self.GetLocalInt("current_tick") + 1;
                        Self.SetLocalInt("current_tick", ticks);

                        if (ticks % 5 == 0)
                        {
                            Self.AssignCommand(() =>
                            {
                                _.ActionPlayAnimation(NWScript.ANIMATION_FIREFORGET_GREETING);
                            });
                            return BehaviourTreeStatus.Success;
                        }

                        return BehaviourTreeStatus.Failure;
                    })
                    .End()
                    .Build();
            }
        }
    }
}
