using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Placeable.TrashCan
{
    public class OnDisturbed: IRegisteredEvent
    {
        private readonly INWScript _;

        public OnDisturbed(INWScript script)
        {
            _ = script;
        }

        public bool Run(params object[] args)
        {
            NWItem oItem = NWItem.Wrap(_.GetInventoryDisturbItem());
            int type = _.GetInventoryDisturbType();

            if (type == INVENTORY_DISTURB_TYPE_ADDED)
            {
                oItem.Destroy();
            }
            return true;
        }
    }
}
