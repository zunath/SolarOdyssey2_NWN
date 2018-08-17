using System.Linq;
using Freescape.Game.Server.Data.Contracts;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Placeable.OverflowStorage
{
    public class OnDisturbed: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IDataContext _db;

        public OnDisturbed(INWScript script,
            IDataContext db)
        {
            _ = script;
            _db = db;
        }

        public bool Run(params object[] args)
        {
            NWPlaceable container = NWPlaceable.Wrap(Object.OBJECT_SELF);
            NWPlayer oPC = NWPlayer.Wrap(_.GetLastDisturbed());
            NWItem oItem = NWItem.Wrap(_.GetInventoryDisturbItem());
            int type = _.GetInventoryDisturbType();

            if (type == INVENTORY_DISTURB_TYPE_ADDED)
            {
                container.AssignCommand(() => _.ActionGiveItem(oItem.Object, oPC.Object));
                return true;
            }
            
            int overflowItemID = oItem.GetLocalInt("TEMP_OVERFLOW_ITEM_ID");
            PCOverflowItem overflowItem = _db.PCOverflowItems.Single(x => x.PCOverflowItemID == overflowItemID);
            _db.PCOverflowItems.Remove(overflowItem);
            _db.SaveChanges();

            oItem.DeleteLocalInt("TEMP_OVERFLOW_ITEM_ID");

            if (container.InventoryItems.Count <= 0)
            {
                container.Destroy();
            }
            return true;
        }
    }
}
