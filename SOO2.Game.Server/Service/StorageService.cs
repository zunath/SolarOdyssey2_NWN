﻿using System.Linq;
using Freescape.Game.Server.Data.Contracts;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Service
{
    public class StorageService : IStorageService
    {
        private readonly INWScript _;
        private readonly IDataContext _db;
        private readonly ISerializationService _serialization;
        private readonly IColorTokenService _color;

        public StorageService(INWScript script, 
            IDataContext db,
            ISerializationService serialization,
            IColorTokenService color)
        {
            _ = script;
            _db = db;
            _serialization = serialization;
            _color = color;
        }

        public void OnChestOpened(NWPlaceable oChest)
        {
            NWArea oArea = oChest.Area;
            int containerID = oChest.GetLocalInt("STORAGE_CONTAINER_ID");
            if (containerID <= 0) return;

            StorageContainer entity = _db.StorageContainers.SingleOrDefault(x => x.StorageContainerID == containerID);
            bool chestLoaded = oChest.GetLocalInt("STORAGE_CONTAINER_LOADED") == 1;

            if (chestLoaded) return;

            if (entity == null)
            {
                entity = new StorageContainer
                {
                    AreaName = oArea.Name,
                    AreaResref = oArea.Resref,
                    AreaTag = oArea.Tag,
                    StorageContainerID = containerID
                };
                _db.StorageContainers.Add(entity);
                _db.SaveChanges();
            }

            foreach (StorageItem item in entity.StorageItems)
            {
                _serialization.DeserializeItem(item.ItemObject, oChest);
            }

            oChest.SetLocalInt("STORAGE_CONTAINER_LOADED", 1);
        }

        public void OnChestDisturbed(NWPlaceable oChest)
        {
            int containerID = oChest.GetLocalInt("STORAGE_CONTAINER_ID");
            if (containerID <= 0) return;

            NWPlayer oPC = NWPlayer.Wrap(_.GetLastDisturbed());
            NWItem oItem = NWItem.Wrap(_.GetInventoryDisturbItem());
            int disturbType = _.GetInventoryDisturbType();
            int itemCount = CountItems(oChest);
            int itemLimit = oChest.GetLocalInt("STORAGE_CONTAINER_ITEM_LIMIT");
            if (itemLimit <= 0) itemLimit = 20;

            StorageContainer entity = _db.StorageContainers.Single(x => x.StorageContainerID == containerID);

            if (disturbType == INVENTORY_DISTURB_TYPE_ADDED)
            {
                if (itemCount > itemLimit)
                {
                    ReturnItem(oPC, oItem);
                    oPC.SendMessage(_color.Red("No more items can be placed inside."));
                }
                else
                {
                    StorageItem itemEntity = new StorageItem
                    {
                        ItemName = oItem.Name,
                        ItemTag = oItem.Tag,
                        ItemResref = oItem.Resref,
                        GlobalID = oItem.GlobalID,
                        ItemObject = _serialization.Serialize(oItem),
                        StorageContainerID = entity.StorageContainerID
                    };

                    entity.StorageItems.Add(itemEntity);
                    _db.SaveChanges();
                }
            }
            else if (disturbType == INVENTORY_DISTURB_TYPE_REMOVED)
            {
                var record = _db.StorageItems.Single(x => x.GlobalID == oItem.GlobalID);
                _db.StorageItems.Remove(record);
                _db.SaveChanges();
            }

            oPC.SendMessage(_color.White("Item Limit: " + itemCount + " / ") + _color.Red("" + itemLimit));
        }

        private void ReturnItem(NWObject oPC, NWObject oItem)
        {
            _.CopyItem(oItem.Object, oPC.Object, TRUE);
            oItem.Destroy();
        }

        private int CountItems(NWObject container)
        {
            int count = 0;

            NWItem item = NWItem.Wrap(_.GetFirstItemInInventory(container.Object));
            while (item.IsValid)
            {
                count++;
                item = NWItem.Wrap(_.GetNextItemInInventory(container.Object));
            }

            return count;
        }
    }
}
