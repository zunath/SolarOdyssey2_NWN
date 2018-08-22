using NWN;
using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Placeable.Resource
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
