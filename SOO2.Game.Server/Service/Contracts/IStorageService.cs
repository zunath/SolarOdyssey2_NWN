using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface IStorageService
    {
        void OnChestDisturbed(NWPlaceable oChest);
        void OnChestOpened(NWPlaceable oChest);
    }
}
