using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Placeable.WarpDevice
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
            NWPlaceable self = NWPlaceable.Wrap(Object.OBJECT_SELF);
            string destination = self.GetLocalString("DESTINATION");
            int visualEffectID = self.GetLocalInt("VISUAL_EFFECT");

            if (visualEffectID > 0)
            {
                _.ApplyEffectToObject(DURATION_TYPE_INSTANT, _.EffectVisualEffect(visualEffectID), oPC.Object);
            }

            oPC.AssignCommand(() =>
            {
                Location location = _.GetLocation(_.GetWaypointByTag(destination));
                _.ActionJumpToLocation(location);
            });

            return true;
        }
    }
}
