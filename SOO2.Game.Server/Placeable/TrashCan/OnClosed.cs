using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using NWN;

namespace SOO2.Game.Server.Placeable.TrashCan
{
    public class OnClosed: IRegisteredEvent
    {
        public bool Run(params object[] args)
        {
            NWPlaceable self = NWPlaceable.Wrap(Object.OBJECT_SELF);
            foreach (NWItem item in self.InventoryItems)
            {
                item.Destroy();
            }

            self.Destroy();
            return true;
        }
    }
}
