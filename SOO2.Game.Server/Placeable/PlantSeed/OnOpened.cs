using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using NWN;
using Object = NWN.Object;

namespace SOO2.Game.Server.Placeable.PlantSeed
{
    public class OnOpened: IRegisteredEvent
    {
        private readonly INWScript _;

        public OnOpened(INWScript script)
        {
            _ = script;
        }

        public bool Run(params object[] args)
        {
            NWPlaceable container = NWPlaceable.Wrap(Object.OBJECT_SELF);
            NWPlayer oPC = NWPlayer.Wrap(_.GetLastUsedBy());
            container.IsUseable = false;
            oPC.SendMessage("Place a seed inside to plant it here. Walk away to cancel planting.");
            return true;
        }
    }
}
