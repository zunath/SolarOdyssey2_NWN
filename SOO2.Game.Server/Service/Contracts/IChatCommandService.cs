using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface IChatCommandService
    {
        void OnModuleNWNXChat(NWPlayer sender);
    }
}
