using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using NWN;

namespace Freescape.Game.Server.Placeable.OverflowStorage
{
    public class OnClosed: IRegisteredEvent
    {
        public bool Run(params object[] args)
        {
            NWPlaceable container = NWPlaceable.Wrap(Object.OBJECT_SELF);
            container.DestroyAllInventoryItems();
            container.Destroy();

            return true;
        }
    }
}
