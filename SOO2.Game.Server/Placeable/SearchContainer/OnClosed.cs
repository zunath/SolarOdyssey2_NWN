using NWN;
using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Placeable.SearchContainer
{
    public class OnClosed: IRegisteredEvent
    {
        private readonly ISearchService _search;

        public OnClosed(ISearchService search)
        {
            _search = search;
        }

        public bool Run(params object[] args)
        {
            _search.OnChestClose(NWPlaceable.Wrap(Object.OBJECT_SELF));
            return true;
        }
    }
}
