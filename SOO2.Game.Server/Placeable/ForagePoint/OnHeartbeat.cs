using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using NWN;

namespace Freescape.Game.Server.Placeable.ForagePoint
{
    public class OnHeartbeat: IRegisteredEvent
    {
        public bool Run(params object[] args)
        {
            NWPlaceable point = NWPlaceable.Wrap(Object.OBJECT_SELF);
            bool isFullyHarvested = point.GetLocalInt("FORAGE_POINT_FULLY_HARVESTED") == 1;
            if (isFullyHarvested)
            {
                int despawnTicks = point.GetLocalInt("FORAGE_POINT_REFILL_TICKS") - 1;
                if (despawnTicks <= 0) despawnTicks = 0;

                point.SetLocalInt("FORAGE_POINT_REFILL_TICKS", despawnTicks);
            }

            return true;
        }
    }
}
