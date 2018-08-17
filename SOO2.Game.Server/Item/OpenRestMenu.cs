using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Item.Contracts;
using SOO2.Game.Server.Service.Contracts;
using SOO2.Game.Server.ValueObject;
using NWN;

namespace SOO2.Game.Server.Item
{
    public class OpenRestMenu: IActionItem
    {
        private readonly INWScript _;
        private readonly IDialogService _dialog;

        public OpenRestMenu(INWScript script,
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
            user.ClearAllActions();

            _dialog.StartConversation((NWPlayer) user, user, "RestMenu");
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
