using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.Placeable.SearchContainer
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
