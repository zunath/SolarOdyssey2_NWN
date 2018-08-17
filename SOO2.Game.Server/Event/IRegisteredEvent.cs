namespace Freescape.Game.Server.Event
{
    internal interface IRegisteredEvent
    {
        bool Run(params object[] args);
    }
}
