﻿using NWN;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using System.Collections.Generic;
using System.Linq;
using static NWN.NWScript;


namespace SOO2.Game.Server.Placeable.CraftingDevice
{
    public class OnDisturbed: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly ICraftService _craft;
        private readonly IItemService _item;
        private readonly IDialogService _dialog;

        public OnDisturbed(INWScript script,
            ICraftService craft,
            IItemService item,
            IDialogService dialog)
        {
            _ = script;
            _craft = craft;
            _item = item;
            _dialog = dialog;
        }

        public bool Run(params object[] args)
        {
            int type = _.GetInventoryDisturbType();
            
            if (type == INVENTORY_DISTURB_TYPE_REMOVED)
            {
                HandleRemoveItem();
            }
            else if (type == INVENTORY_DISTURB_TYPE_ADDED)
            {
                HandleAddItem();
            }
            return true;
        }


        private void HandleAddItem()
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetLastDisturbed());
            NWItem oItem = NWItem.Wrap(_.GetInventoryDisturbItem());
            if (oItem.Resref == "cft_confirm") return;
            var model = _craft.GetPlayerCraftingData(oPC);
            NWPlaceable storage = NWPlaceable.Wrap(_.GetObjectByTag("craft_temp_store"));

            List<NWItem> list = null;
            ComponentType allowedType = ComponentType.None;
            bool reachedCap = false;
            string componentName = string.Empty;
            switch (model.Access)
            {
                case CraftingAccessType.MainComponent:
                    allowedType = (ComponentType)model.Blueprint.MainComponentTypeID;
                    reachedCap = model.Blueprint.MainMinimum * 2 < model.MainComponents.Count + 1;
                    list = model.MainComponents;
                    componentName = model.Blueprint.MainComponentType.Name;
                    break;
                case CraftingAccessType.SecondaryComponent:
                    allowedType = (ComponentType)model.Blueprint.SecondaryComponentTypeID;
                    reachedCap = model.Blueprint.SecondaryMinimum * 2 < model.SecondaryComponents.Count + 1;
                    list = model.SecondaryComponents;
                    componentName = model.Blueprint.SecondaryComponentType.Name;
                    break;
                case CraftingAccessType.TertiaryComponent:
                    allowedType = (ComponentType)model.Blueprint.TertiaryComponentTypeID;
                    reachedCap = model.Blueprint.TertiaryMinimum * 2 < model.TertiaryComponents.Count + 1;
                    list = model.TertiaryComponents;
                    componentName = model.Blueprint.TertiaryComponentType.Name;
                    break;
                case CraftingAccessType.Enhancement:
                    allowedType = ComponentType.Enhancement;
                    reachedCap = model.Blueprint.EnhancementSlots < model.EnhancementComponents.Count + 1;
                    list = model.EnhancementComponents;
                    componentName = "Enhancement";
                    break;
            }

            if (list == null)
            {
                oPC.FloatingText("There was an issue getting the item data. Notify an admin.");
                _item.ReturnItem(oPC, oItem);
                return;
            }

            if (reachedCap)
            {
                _item.ReturnItem(oPC, oItem);
                oPC.FloatingText("You cannot add any more components of that type.");
                return;
            }

            foreach (var ip in oItem.ItemProperties)
            {
                if (_.GetItemPropertyType(ip) == (int) CustomItemPropertyType.ComponentType)
                {
                    int compType = _.GetItemPropertyCostTableValue(ip);
                    if (compType == (int) allowedType)
                    {
                        NWItem copy = NWItem.Wrap(_.CopyItem(oItem.Object, storage.Object, TRUE));
                        list.Add(copy);
                        return;
                    }
                }
            }

            oPC.FloatingText("Only " + componentName + " components may be used with this component type.");
            _item.ReturnItem(oPC, oItem);
        }

        private void HandleRemoveItem()
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetLastDisturbed());
            NWItem oItem = NWItem.Wrap(_.GetInventoryDisturbItem());
            NWPlaceable device = NWPlaceable.Wrap(Object.OBJECT_SELF);
            NWPlaceable storage = NWPlaceable.Wrap(_.GetObjectByTag("craft_temp_store"));
            var model = _craft.GetPlayerCraftingData(oPC);
            
            if (oItem.Resref == "cft_confirm")
            {
                oItem.Destroy();
                device.DestroyAllInventoryItems();
                device.IsLocked = false;
                model.IsAccessingStorage = false;
                _dialog.StartConversation(oPC, device, "CraftItem");
                return;
            }

            List<NWItem> items = null;

            switch(model.Access)
            {
                case CraftingAccessType.MainComponent:
                    items = model.MainComponents;
                    break;
                case CraftingAccessType.SecondaryComponent:
                    items = model.SecondaryComponents;
                    break;
                case CraftingAccessType.TertiaryComponent:
                    items = model.TertiaryComponents;
                    break;
                case CraftingAccessType.Enhancement:
                    items = model.EnhancementComponents;
                    break;
            }

            NWItem copy = storage.InventoryItems.SingleOrDefault(x => x.GlobalID == oItem.GlobalID);
            NWItem listItem = items?.SingleOrDefault(x => x.GlobalID == oItem.GlobalID);
            if (listItem == null || copy == null || !copy.IsValid) return;

            copy.Destroy();
            items.Remove(listItem);

        }
        
    }
}
