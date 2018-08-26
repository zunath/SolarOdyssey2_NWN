using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IItemService
    {
        int[] ArmorBaseItemTypes { get; }
        int[] WeaponBaseItemTypes { get; }

        string GetNameByResref(string resref);
        void OnModuleActivatedItem();
        void OnModuleEquipItem();
        string OnModuleExamine(string existingDescription, NWPlayer examiner, NWObject examinedObject);
        void OnModuleHeartbeat();
        void ReturnItem(NWObject target, NWItem item);
        void StripAllItemProperties(NWItem item);
    }
}
