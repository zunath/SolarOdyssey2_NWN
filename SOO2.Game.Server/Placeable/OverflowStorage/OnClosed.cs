using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using NWN;

namespace SOO2.Game.Server.Placeable.OverflowStorage
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
