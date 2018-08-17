using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using NWN;

namespace SOO2.Game.Server.Placeable.SearchContainer
{
    public class OnOpened : IRegisteredEvent
    {
        private readonly ISearchService _search;

        public OnOpened(ISearchService search)
        {
            _search = search;
        }

        public bool Run(params object[] args)
        {
            _search.OnChestOpen(NWPlaceable.Wrap(Object.OBJECT_SELF));
            return true;
        }
    }
}
