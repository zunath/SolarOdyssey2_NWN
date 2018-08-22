using NWN;
using SOO2.Game.Server.Data.Entities;
using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Placeable.CraftingDevice
{
    public class OnOpened: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly ICraftService _craft;

        public OnOpened(INWScript script,
            ICraftService craft)
        {
            _ = script;
            _craft = craft;
        }

        public bool Run(params object[] args)
        {
            NWPlaceable device = NWPlaceable.Wrap(Object.OBJECT_SELF);
            NWPlayer oPC = NWPlayer.Wrap(_.GetLastOpenedBy());
            int blueprintSelected = device.GetLocalInt("CRAFT_BLUEPRINT_ID");

            _.CreateItemOnObject("cft_choose_bp", device.Object);
            if (blueprintSelected > 0)
            {
                CraftBlueprint entity = _craft.GetBlueprintByID(blueprintSelected);

                NWItem menuItem = NWItem.Wrap(_.CreateItemOnObject("cft_craft_item", device.Object));
                menuItem.Name = "Craft Item: " + entity.ItemName;

                oPC.SendMessage(_craft.BuildBlueprintHeader(oPC, blueprintSelected));
            }

            return true;
        }
    }
}
