using NWN;
using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Placeable.StorageContainer
{
    public class OnOpened : IRegisteredEvent
    {
        private readonly IStorageService _storage;

        public OnOpened(IStorageService storage)
        {
            _storage = storage;
        }

        public bool Run(params object[] args)
        {
            _storage.OnChestOpened(NWPlaceable.Wrap(Object.OBJECT_SELF));
            return true;
        }
    }
}
