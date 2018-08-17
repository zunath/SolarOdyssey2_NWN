using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.Placeable.StorageContainer
{
    public class OnDisturbed : IRegisteredEvent
    {
        private readonly IStorageService _storage;

        public OnDisturbed(IStorageService storage)
        {
            _storage = storage;
        }

        public bool Run(params object[] args)
        {
            _storage.OnChestDisturbed(NWPlaceable.Wrap(Object.OBJECT_SELF));
            return true;
        }
    }
}
