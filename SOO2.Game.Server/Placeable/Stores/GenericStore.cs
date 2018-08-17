using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using NWN;
using Object = NWN.Object;

namespace SOO2.Game.Server.Placeable.Stores
{
    public class GenericStore: IRegisteredEvent
    {
        private readonly INWScript _;

        public GenericStore(INWScript script)
        {
            _ = script;
        }

        public bool Run(params object[] args)
        {
            NWPlaceable self = NWPlaceable.Wrap(Object.OBJECT_SELF);
            NWObject oPC = NWObject.Wrap(_.GetLastUsedBy());
            string storeTag = self.GetLocalString("STORE_TAG");
            Object store = _.GetObjectByTag(storeTag);

            _.OpenStore(store, oPC.Object);

            return true;
        }
    }
}
