using NWN;
using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Placeable.SearchContainer
{
    public class OnDisturbed: IRegisteredEvent
    {
        private readonly ISearchService _search;

        public OnDisturbed(ISearchService search)
        {
            _search = search;
        }

        public bool Run(params object[] args)
        {
            _search.OnChestDisturbed(NWPlaceable.Wrap(Object.OBJECT_SELF));
            return true;
        }
    }
}
