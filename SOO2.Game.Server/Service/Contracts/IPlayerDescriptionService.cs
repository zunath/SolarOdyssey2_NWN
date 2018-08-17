using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IPlayerDescriptionService
    {
        void OnModuleChat();
        void ChangePlayerDescription(NWPlayer player);
    }
}
