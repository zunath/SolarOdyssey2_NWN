using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface IActivityLoggingService
    {
        void OnModuleClientEnter();
        void OnModuleClientLeave();
        void OnModuleNWNXChat(NWPlayer sender);
    }
}
