using NWN;
using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Placeable.PlantSeed
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
