﻿namespace SOO2.Game.Server.Event.Module
{
    internal class OnModuleCutsceneAbort : IRegisteredEvent
    {
        public bool Run(params object[] args)
        {
            return true;

        }
    }
}
