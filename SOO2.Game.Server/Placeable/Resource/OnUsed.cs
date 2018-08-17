using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using NWN;

namespace Freescape.Game.Server.Placeable.Resource
{
    public class OnUsed: IRegisteredEvent
    {
        private readonly INWScript _;

        public OnUsed(INWScript script)
        {
            _ = script;
        }

        public bool Run(params object[] args)
        {
            NWObject oPC = NWObject.Wrap(_.GetLastUsedBy());
            oPC.AssignCommand(() => _.ActionAttack(Object.OBJECT_SELF));
            return true;
        }
    }
}
