﻿using FluentBehaviourTree;
using NWN;
using SOO2.Game.Server.AI.AIComponent;
using SOO2.Game.Server.Extension;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.AI
{
    public class StandardBehaviour : BehaviourBase
    {
        private readonly INWScript _;
        protected readonly BehaviourTreeBuilder _builder;
        private readonly IEnmityService _enmity;
        private readonly IDialogService _dialog;

        public StandardBehaviour(BehaviourTreeBuilder builder,
            INWScript script,
            IEnmityService enmity,
            IDialogService dialog)
        {
            _ = script;
            _builder = builder;
            _enmity = enmity;
            _dialog = dialog;
        }

        public override bool IgnoreNWNEvents => true;

        public override BehaviourTreeBuilder Behaviour => _builder
            .Parallel("StandardBehaviour", 5, 1)
            .Do<CleanUpEnmity>(Self)
            .Do<AttackHighestEnmity>(Self);

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

        public override void OnConversation()
        {
            base.OnConversation();
            string convo = Self.GetLocalString("CONVERSATION");

            if (!string.IsNullOrWhiteSpace(convo))
            {
                NWPlayer player = NWPlayer.Wrap(_.GetLastSpeaker());
                _dialog.StartConversation(player, Self, convo);
            }
        }

    }
}