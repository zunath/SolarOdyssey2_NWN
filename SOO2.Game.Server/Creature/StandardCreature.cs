using FluentBehaviourTree;
using NWN;
using SOO2.Game.Server.BehaviourComponent;
using SOO2.Game.Server.Extension;
using SOO2.Game.Server.Service.Contracts;

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

        public override IBehaviourTreeNode Behaviour => _builder
            .Parallel("StandardCreature", 1, 2)
            .Do<CleanUpEnmity>(Self)
            .Do<AttackHighestEnmity>(Self)
            .End()
            .Build();

        public override void OnPhysicalAttacked()
        {
            base.OnPhysicalAttacked();
            _enmity.OnNPCPhysicallyAttacked();
        }

        public override void OnDamaged()
        {
            base.OnDamaged();
            _enmity.OnNPCDamaged();
        }

    }
}
