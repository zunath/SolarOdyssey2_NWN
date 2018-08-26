﻿using NWN;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Event.Module
{
    internal class OnModuleUseFeat : IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IAbilityService _ability;
        private readonly IStructureService _structure;
        private readonly IPlayerService _player;

        public OnModuleUseFeat(
            INWScript script,
            IAbilityService ability,
            IStructureService structure,
            IPlayerService player)
        {
            _ = script;
            _ability = ability;
            _structure = structure;
            _player = player;
        }

        public bool Run(params object[] args)
        {
            _ability.OnModuleUseFeat();
            _structure.OnModuleUseFeat();
            _player.OnModuleUseFeat();
            return true;

        }
    }
}
