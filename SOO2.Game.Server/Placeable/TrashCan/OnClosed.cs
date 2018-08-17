using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using NWN;

namespace Freescape.Game.Server.Placeable.TrashCan
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
