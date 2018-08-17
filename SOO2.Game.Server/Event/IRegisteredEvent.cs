namespace SOO2.Game.Server.Event
{
    internal interface IRegisteredEvent
    {
        bool Run(params object[] args);
    }
}
