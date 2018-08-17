using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Item.Contracts;
using Freescape.Game.Server.Service.Contracts;
using Freescape.Game.Server.ValueObject;
using NWN;

namespace Freescape.Game.Server.Item
{
    public class StructureTool: IActionItem
    {
        private readonly IStructureService _structure;
        private readonly IDialogService _dialog;

        public StructureTool(IStructureService structure,
            IDialogService dialog)
        {
            _structure = structure;
            _dialog = dialog;
        }

        public CustomData StartUseItem(NWCreature user, NWItem item, NWObject target, Location targetLocation)
        {
            return null;
        }

        public void ApplyEffects(NWCreature user, NWItem item, NWObject target, Location targetLocation, CustomData customData)
        {
            bool isMovingStructure = _structure.IsPCMovingStructure((NWPlayer)user);

            if (isMovingStructure)
            {
                _structure.MoveStructure((NWPlayer)user, targetLocation);
            }
            else
            {
                user.SetLocalLocation("BUILD_TOOL_LOCATION_TARGET", targetLocation);
                _dialog.StartConversation(user, user, "BuildToolMenu");
            }
        }

        public float Seconds(NWCreature user, NWItem item, NWObject target, Location targetLocation, CustomData customData)
        {
            return 0;
        }

        public bool FaceTarget()
        {
            return false;
        }

        public int AnimationID()
        {
            return 0;
        }

        public float MaxDistance()
        {
            return 0;
        }

        public bool ReducesItemCharge(NWCreature user, NWItem item, NWObject target, Location targetLocation, CustomData customData)
        {
            return false;
        }

        public string IsValidTarget(NWCreature user, NWItem item, NWObject target, Location targetLocation)
        {
            return null;
        }

        public bool AllowLocationTarget()
        {
            return true;
        }
    }
}
