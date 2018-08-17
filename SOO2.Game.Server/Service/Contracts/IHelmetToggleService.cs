using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface IHelmetToggleService
    {
        void OnModuleItemEquipped();
        void OnModuleItemUnequipped();
        void ToggleHelmetDisplay(NWPlayer player);
    }
}
