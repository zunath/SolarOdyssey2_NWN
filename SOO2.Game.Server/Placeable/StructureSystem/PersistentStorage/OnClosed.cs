﻿using NWN;
using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Placeable.StructureSystem.PersistentStorage
{
    public class OnClosed: IRegisteredEvent
    {
        public bool Run(params object[] args)
        {
            NWPlaceable objSelf = NWPlaceable.Wrap(Object.OBJECT_SELF);

            foreach (NWItem item in objSelf.InventoryItems)
            {
                item.Destroy();
            }

            NWObject parent = NWObject.Wrap(objSelf.GetLocalObject("STRUCTURE_TEMP_PARENT"));
            parent.DeleteLocalObject("STRUCTURE_TEMP_INVENTORY_OPENED");

            objSelf.Destroy();
            return true;
        }
    }
}
