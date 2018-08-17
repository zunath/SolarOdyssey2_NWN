using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using NWN;

namespace Freescape.Game.Server.Placeable
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
