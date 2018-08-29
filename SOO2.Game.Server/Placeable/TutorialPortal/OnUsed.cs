using NWN;
using SOO2.Game.Server.Data.Entities;
using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Placeable.TutorialPortal
{
    public class OnUsed: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IPlayerService _player;

        public OnUsed(
            INWScript script,
            IPlayerService player)
        {
            _ = script;
            _player = player;
        }

        public bool Run(params object[] args)
        {
            NWPlayer player = NWPlayer.Wrap(_.GetLastUsedBy());
            NWPlaceable warp = NWPlaceable.Wrap(Object.OBJECT_SELF);
            bool isExit = warp.GetLocalInt("IS_EXIT") == NWScript.TRUE;

            if (isExit)
            {
                PlayerCharacter entity = _player.GetPlayerEntity(player.GlobalID);
                NWArea area = NWArea.Wrap(_.GetObjectByTag(entity.LocationAreaTag));
                Vector position = _.Vector((float) entity.LocationX, (float) entity.LocationY, (float) entity.LocationZ);
                Location location = _.Location(area.Object,
                    position,
                    (float) entity.LocationOrientation);

                player.AssignCommand(() => _.ActionJumpToLocation(location));
            }
            else
            {
                _player.SaveLocation(player);
                NWObject waypoint = NWObject.Wrap(_.GetWaypointByTag("TUTORIAL_WP"));
                player.AssignCommand(() => _.ActionJumpToLocation(waypoint.Location));
            }


            return true;
        }
    }
}
