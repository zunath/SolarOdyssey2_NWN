using System.Collections.Generic;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface IKeyItemService
    {
        KeyItem GetKeyItemByID(int keyItemID);
        IEnumerable<PCKeyItem> GetPlayerKeyItemsByCategory(NWPlayer player, int categoryID);
        void GivePlayerKeyItem(NWPlayer oPC, int keyItemID);
        void OnModuleItemAcquired();
        bool PlayerHasKeyItem(NWObject oPC, int keyItemID);
        void RemovePlayerKeyItem(NWPlayer oPC, int keyItemID);
    }
}
