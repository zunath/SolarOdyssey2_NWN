using NWN;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.ValueObject;

namespace SOO2.Game.Server.Item.Contracts
{
    public interface IActionItem
    {
        CustomData StartUseItem(NWCreature user, NWItem item, NWObject target, Location targetLocation);
        void ApplyEffects(NWCreature user, NWItem item, NWObject target, Location targetLocation, CustomData customData);
        float Seconds(NWCreature user, NWItem item, NWObject target, Location targetLocation, CustomData customData);
        bool FaceTarget();
        int AnimationID();
        float MaxDistance();
        bool ReducesItemCharge(NWCreature user, NWItem item, NWObject target, Location targetLocation, CustomData customData);
        string IsValidTarget(NWCreature user, NWItem item, NWObject target, Location targetLocation);
        bool AllowLocationTarget();
    }
}
