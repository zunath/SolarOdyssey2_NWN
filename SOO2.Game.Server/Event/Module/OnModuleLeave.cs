﻿using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.Event.Module
{
    internal class OnModuleLeave : IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IPlayerService _player;
        private readonly IActivityLoggingService _activityLogging;
        private readonly ISkillService _skill;
        private readonly IMapPinService _mapPin;

        public OnModuleLeave(
            INWScript script,
            IPlayerService player,
            IActivityLoggingService activityLogging,
            ISkillService skill,
            IMapPinService mapPin)
        {
            _ = script;
            _player = player;
            _activityLogging = activityLogging;
            _skill = skill;
            _mapPin = mapPin;
        }

        public bool Run(params object[] args)
        {
            NWPlayer pc = NWPlayer.Wrap(_.GetExitingObject());

            if (pc.IsPlayer)
            {
                _.ExportSingleCharacter(pc.Object);
            }

            _player.SaveCharacter(pc);
            _activityLogging.OnModuleClientLeave();
            _skill.OnModuleClientLeave();
            _mapPin.OnModuleClientLeave();
            return true;

        }
    }
}
