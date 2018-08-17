﻿using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Event.Area
{
    internal class OnAreaExit: IRegisteredEvent
    {
        private readonly ISkillService _skill;

        public OnAreaExit(ISkillService skill)
        {
            _skill = skill;
        }

        public bool Run(params object[] args)
        {
            _skill.OnAreaExit();
            return true;
        }
    }
}
