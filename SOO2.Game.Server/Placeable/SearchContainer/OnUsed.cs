using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.Placeable.SearchContainer
{
    public class OnUsed : IRegisteredEvent
    {
        private readonly ISearchService _search;

        public OnUsed(ISearchService search)
        {
            _search = search;
        }

        public bool Run(params object[] args)
        {
            _search.OnChestUsed(NWPlaceable.Wrap(Object.OBJECT_SELF));
            return true;
        }
    }
}
