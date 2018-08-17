using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Placeable.CraftingDevice
{
    public class OnDisturbed: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly ICraftService _craft;
        private readonly IColorTokenService _color;
        private readonly IItemService _item;
        private readonly IDialogService _dialog;

        public OnDisturbed(INWScript script,
            ICraftService craft,
            IColorTokenService color,
            IItemService item,
            IDialogService dialog)
        {
            _ = script;
            _craft = craft;
            _color = color;
            _item = item;
            _dialog = dialog;
        }

        public bool Run(params object[] args)
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetLastDisturbed());
            NWItem oItem = NWItem.Wrap(_.GetInventoryDisturbItem());
            NWPlaceable device = NWPlaceable.Wrap(Object.OBJECT_SELF);

            int type = _.GetInventoryDisturbType();
            string resref = oItem.Resref;
            string tag = oItem.Tag;
            int blueprintID = device.GetLocalInt("CRAFT_BLUEPRINT_ID");
            NWItem tools = NWItem.Wrap(device.GetLocalObject("CRAFT_DEVICE_TOOLS"));
            CraftBlueprint blueprint = _craft.GetBlueprintByID(blueprintID);

            if (type == INVENTORY_DISTURB_TYPE_REMOVED)
            {
                if (resref == "cft_choose_bp" || resref == "cft_craft_item")
                {
                    _item.ReturnItem(device, oItem);

                    if (tag == "CHOOSE_BLUEPRINT")
                    {
                        _dialog.StartConversation(oPC, device, "Crafting");
                    }
                    else if (tag == "CRAFT_ITEM")
                    {
                        HandleCraftItem(oPC, device);
                    }
                }
                else if (Equals(oItem, tools))
                {
                    device.DeleteLocalObject("CRAFT_DEVICE_TOOLS");
                }
            }
            else if (type == INVENTORY_DISTURB_TYPE_ADDED)
            {
                if (resref == "cft_choose_bp" || resref == "cft_craft_item") return false;

                if (blueprint == null)
                {
                    _item.ReturnItem(oPC, oItem);
                    oPC.SendMessage("Please select a blueprint before adding components.");
                }
                else if (oItem.CraftTierLevel > 0)
                {
                    if (blueprint.CraftTierLevel <= 0)
                    {
                        _item.ReturnItem(oPC, oItem);
                        oPC.SendMessage("Tools are not required to make this item.");
                    }
                    else if (blueprint.CraftTierLevel > oItem.CraftTierLevel || blueprint.SkillID != oItem.AssociatedSkillID)
                    {
                        _item.ReturnItem(oPC, oItem);
                        oPC.SendMessage("Those tools cannot be used with this blueprint. (Required Tool Level: " + blueprint.CraftTierLevel + ")");
                    }
                    else
                    {
                        if (tools.IsValid)
                        {
                            _item.ReturnItem(oPC, oItem);
                            oPC.SendMessage("You may only use one set of tools at a time.");
                        }
                        else
                        {
                            device.SetLocalObject("CRAFT_DEVICE_TOOLS", oItem.Object);
                        }
                    }
                }
            }
            return true;
        }


        private void HandleCraftItem(NWPlayer oPC, NWPlaceable device)
        {
            int blueprintID = device.GetLocalInt("CRAFT_BLUEPRINT_ID");
            int deviceID = device.GetLocalInt("CRAFT_DEVICE_ID");
            CraftBlueprint pcBlueprint = _craft.GetBlueprintKnownByPC(oPC.GlobalID, blueprintID, deviceID);

            if (pcBlueprint == null)
            {
                oPC.FloatingText(_color.Red("You do not know that blueprint."));
                return;
            }

            _craft.CraftItem(oPC, device, blueprintID);
        }

    }
}
