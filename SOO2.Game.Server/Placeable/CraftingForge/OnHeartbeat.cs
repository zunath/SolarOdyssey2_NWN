using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using NWN;

namespace Freescape.Game.Server.Placeable.CraftingForge
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
