using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IStorageService
    {
        void OnChestDisturbed(NWPlaceable oChest);
        void OnChestOpened(NWPlaceable oChest);
    }
}
