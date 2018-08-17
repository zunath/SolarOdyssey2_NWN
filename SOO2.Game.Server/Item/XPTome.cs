using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Item.Contracts;
using Freescape.Game.Server.Service.Contracts;
using Freescape.Game.Server.ValueObject;
using NWN;

namespace Freescape.Game.Server.Item
{
    public class XPTome: IActionItem
    {
        private readonly INWScript _;
        private readonly IDialogService _dialog;

        public XPTome(INWScript script,
            IDialogService dialog)
        {
            _ = script;
            _dialog = dialog;
        }

        public CustomData StartUseItem(NWCreature user, NWItem item, NWObject target, Location targetLocation)
        {
            return null;
        }

        public void ApplyEffects(NWCreature user, NWItem item, NWObject target, Location targetLocation, CustomData customData)
        {
            user.SetLocalObject("XP_TOME_OBJECT", item.Object);
            user.ClearAllActions();
            
            _dialog.StartConversation(user, user, "XPTome");
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
            return false;
        }
    }
}
