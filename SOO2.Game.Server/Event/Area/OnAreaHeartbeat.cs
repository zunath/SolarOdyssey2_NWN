namespace SOO2.Game.Server.Event.Area
{
    internal class OnAreaHeartbeat: IRegisteredEvent
    {
        public bool Run(params object[] args)
        {
            return true;

        }
    }
}
