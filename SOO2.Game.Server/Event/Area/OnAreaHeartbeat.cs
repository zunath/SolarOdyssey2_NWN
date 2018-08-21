﻿using NWN;

namespace SOO2.Game.Server.Event.Area
{
    internal class OnAreaHeartbeat: IRegisteredEvent
    {
        private readonly INWScript _;

        public OnAreaHeartbeat(INWScript script)
        {
            _ = script;
        }

        public bool Run(params object[] args)
        {
            _.ExecuteScript("spawn_sample_hb", Object.OBJECT_SELF);

            return true;

        }
    }
}
