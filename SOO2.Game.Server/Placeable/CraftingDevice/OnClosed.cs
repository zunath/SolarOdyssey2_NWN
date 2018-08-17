using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using NWN;
using static NWN.NWScript;
using Object = NWN.Object;

namespace Freescape.Game.Server.Placeable.CraftingDevice
{
    public class OnClosed: IRegisteredEvent
    {
        private readonly INWScript _;

        public OnClosed(INWScript script)
        {
            _ = script;
        }

        public bool Run(params object[] args)
        {
            NWPlaceable device = NWPlaceable.Wrap(Object.OBJECT_SELF);
            NWObject oPC = NWObject.Wrap(_.GetLastClosedBy());

            foreach (NWItem item in device.InventoryItems)
            {
                string resref = item.Resref;

                if (resref != "cft_choose_bp" && resref != "cft_craft_item")
                {
                    _.CopyItem(item.Object, oPC.Object, TRUE);
                }
                item.Destroy();
            }

            return true;
        }
    }
}
