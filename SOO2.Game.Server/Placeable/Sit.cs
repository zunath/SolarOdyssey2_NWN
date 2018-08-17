using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using NWN;

namespace SOO2.Game.Server.Placeable
{
    public class Sit: IRegisteredEvent
    {
        private readonly INWScript _;

        public Sit(INWScript script)
        {
            _ = script;
        }

        public bool Run(params object[] args)
        {
            NWObject user = NWObject.Wrap(_.GetLastUsedBy());
            user.AssignCommand(() =>
            {
                _.ActionSit(Object.OBJECT_SELF);
            });

            return true;
        }
    }
}
