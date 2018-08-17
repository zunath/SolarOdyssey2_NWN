using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.Placeable.PlantSeed
{
    public class OnClosed: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IItemService _item;

        public OnClosed(INWScript script, IItemService item)
        {
            _ = script;
            _item = item;
        }

        public bool Run(params object[] args)
        {
            NWPlaceable container = NWPlaceable.Wrap(Object.OBJECT_SELF);
            NWPlayer oPC = NWPlayer.Wrap(_.GetLastClosedBy());
            
            foreach (NWItem item in container.InventoryItems)
            {
                _item.ReturnItem(oPC, item);
            }

            container.Destroy();
            return true;
        }
    }
}
