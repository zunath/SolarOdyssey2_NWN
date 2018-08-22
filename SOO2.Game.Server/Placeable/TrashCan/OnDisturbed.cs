using NWN;
using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using static NWN.NWScript;

namespace SOO2.Game.Server.Placeable.TrashCan
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
