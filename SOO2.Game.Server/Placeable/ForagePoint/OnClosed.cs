using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using Object = NWN.Object;

namespace Freescape.Game.Server.Placeable.ForagePoint
{
    public class OnClosed: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IFarmingService _farming;

        public OnClosed(INWScript script,
            IFarmingService farming)
        {
            _ = script;
            _farming = farming;
        }

        public bool Run(params object[] args)
        {
            NWPlaceable point = NWPlaceable.Wrap(Object.OBJECT_SELF);

            bool isFullyHarvested = point.GetLocalInt("FORAGE_POINT_FULLY_HARVESTED") == 1;

            if (point.InventoryItems.Count <= 0 && isFullyHarvested)
            {
                string seed = point.GetLocalString("FORAGE_POINT_SEED");
                if (!string.IsNullOrWhiteSpace(seed))
                {
                    _.CreateObject(NWScript.OBJECT_TYPE_ITEM, seed, point.Location);
                }

                point.Destroy();
                _farming.RemoveGrowingPlant(point);
            }
            return false;
        }
    }
}
