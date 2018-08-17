using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface IPlayerDescriptionService
    {
        void OnModuleChat();
        void ChangePlayerDescription(NWPlayer player);
    }
}
