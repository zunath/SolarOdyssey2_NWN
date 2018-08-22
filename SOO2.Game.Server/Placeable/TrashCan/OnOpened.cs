using NWN;
using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Placeable.TrashCan
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
            NWPlayer oPC = NWPlayer.Wrap(_.GetLastOpenedBy());
            oPC.FloatingText("Any item placed inside this trash can will be destroyed permanently.");

            return true;
        }
    }
}
