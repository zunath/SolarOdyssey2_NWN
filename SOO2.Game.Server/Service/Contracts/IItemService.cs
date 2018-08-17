using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface IItemService
    {
        string GetNameByResref(string resref);
        void OnModuleItemAcquired();
        void OnModuleActivatedItem();
        void OnModuleHeartbeat();
        string OnModuleExamine(string existingDescription, NWPlayer examiner, NWObject examinedObject);
        void OnModuleEquipItem();
        void ReturnItem(NWObject target, NWItem item);
        void StripAllItemProperties(NWItem item);
    }
}
