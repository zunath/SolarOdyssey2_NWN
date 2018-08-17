using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using NWN;

namespace SOO2.Game.Server.Placeable.CraftingForge
{
    public class OnHeartbeat: IRegisteredEvent
    {
        public bool Run(params object[] args)
        {
            NWPlaceable forge = NWPlaceable.Wrap(Object.OBJECT_SELF);
            int charges = forge.GetLocalInt("FORGE_CHARGES");

            if (charges > 0)
            {
                charges--;
                forge.SetLocalInt("FORGE_CHARGES", charges);
            }

            if (charges <= 0)
            {
                NWPlaceable flames = NWPlaceable.Wrap(forge.GetLocalObject("FORGE_FLAMES"));
                flames.Destroy();
            }
            return true;
        }
    }
}
