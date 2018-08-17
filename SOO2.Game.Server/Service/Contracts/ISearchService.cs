using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface ISearchService
    {
        void OnChestClose(NWPlaceable oChest);
        void OnChestDisturbed(NWPlaceable oChest);
        void OnChestOpen(NWPlaceable oChest);
        void OnChestUsed(NWPlaceable oChest);
    }
}
