using NWN;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using static NWN.NWScript;

namespace SOO2.Game.Server.Service
{
    public class MapService : IMapService
    {
        private readonly INWScript _;

        public MapService(INWScript script)
        {
            _ = script;
        }

        public void OnAreaEnter()
        {
            NWArea area = NWArea.Wrap(Object.OBJECT_SELF);
            if (area.GetLocalInt("AUTO_EXPLORED") == TRUE)
            {
                _.ExploreAreaForPlayer(area.Object, _.GetEnteringObject());
            }

        }

    }
}
