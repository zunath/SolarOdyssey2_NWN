using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using NWN;

namespace SOO2.Game.Server.Placeable.StorageContainer
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
