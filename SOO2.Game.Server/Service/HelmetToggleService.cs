using System;
using System.Linq;
using Freescape.Game.Server.Data.Contracts;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Service
{
    public class HelmetToggleService: IHelmetToggleService
    {
        private readonly IDataContext _db;
        private readonly INWScript _;

        public HelmetToggleService(IDataContext db, INWScript script)
        {
            _db = db;
            _ = script;
        }

        public void OnModuleItemEquipped()
        {
            NWPlayer player = NWPlayer.Wrap(_.GetPCItemLastEquippedBy());
            if (!player.IsPlayer || !player.IsInitializedAsPlayer) return;

            NWItem item = NWItem.Wrap(_.GetPCItemLastEquipped());
            if (item.BaseItemType != BASE_ITEM_HELMET) return;

            PlayerCharacter pc = _db.PlayerCharacters.Single(x => x.PlayerID == player.GlobalID);
            _.SetHiddenWhenEquipped(item.Object, !pc.DisplayHelmet == false ? 0 : 1);
        }

        public void OnModuleItemUnequipped()
        {
            NWPlayer player = NWPlayer.Wrap(_.GetPCItemLastUnequippedBy());
            if (!player.IsPlayer) return;

            NWItem item = NWItem.Wrap(_.GetPCItemLastUnequipped());
            if (item.BaseItemType != BASE_ITEM_HELMET) return;

            PlayerCharacter pc = _db.PlayerCharacters.Single(x => x.PlayerID == player.GlobalID);
            _.SetHiddenWhenEquipped(item.Object, !pc.DisplayHelmet == false ? 0 : 1);
        }

        public void ToggleHelmetDisplay(NWPlayer player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));

            if (!player.IsPlayer) return;

            PlayerCharacter pc = _db.PlayerCharacters.Single(x => x.PlayerID == player.GlobalID);
            pc.DisplayHelmet = !pc.DisplayHelmet;
            _db.SaveChanges();

            _.FloatingTextStringOnCreature(
                pc.DisplayHelmet ? "Now showing equipped helmet." : "Now hiding equipped helmet.", 
                player.Object,
                FALSE);

            NWItem helmet = NWItem.Wrap(_.GetItemInSlot(INVENTORY_SLOT_HEAD, player.Object));
            if (helmet.IsValid)
            {
                _.SetHiddenWhenEquipped(helmet.Object, !pc.DisplayHelmet == false ? 0 : 1);
            }

        }
    }
}
