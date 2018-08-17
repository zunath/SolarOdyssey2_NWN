using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.Placeable.SearchContainer
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
