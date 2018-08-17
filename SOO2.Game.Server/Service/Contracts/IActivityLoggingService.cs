using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IActivityLoggingService
    {
        void OnModuleClientEnter();
        void OnModuleClientLeave();
        void OnModuleNWNXChat(NWPlayer sender);
    }
}
