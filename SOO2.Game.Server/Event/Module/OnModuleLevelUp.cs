namespace SOO2.Game.Server.Event.Module
{
    internal class OnModuleLevelUp : IRegisteredEvent
    {
        public bool Run(params object[] args)
        {
            return true;

        }
    }
}
