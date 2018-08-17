﻿using SOO2.Game.Server.ChatCommands.Contracts;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject;
using NWN;

namespace SOO2.Game.Server.ChatCommands
{
    [CommandDetails("Makes your character sit down.", CommandPermissionType.DM | CommandPermissionType.Player)]
    public class Sit: IChatCommand
    {
        private readonly INWScript _;

        public Sit(INWScript script)
        {
            _ = script;
        }

        public void DoAction(NWPlayer user, params string[] args)
        {
            user.AssignCommand(() => _.ActionPlayAnimation(NWScript.ANIMATION_LOOPING_SIT_CROSS, 1.0f, 9999));
        }
    }
}
