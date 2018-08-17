using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IHelmetToggleService
    {
        void OnModuleItemEquipped();
        void OnModuleItemUnequipped();
        void ToggleHelmetDisplay(NWPlayer player);
    }
}
