using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.Placeable.CraftingDevice
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
