using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using NWN;

namespace Freescape.Game.Server.Placeable.TrashCan
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
