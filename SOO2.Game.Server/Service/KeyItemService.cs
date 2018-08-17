﻿using System.Collections.Generic;
using System.Linq;
using Freescape.Game.Server.Data.Contracts;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.Service
{
    public class KeyItemService : IKeyItemService
    {
        private readonly IDataContext _db;
        private readonly INWScript _;

        public KeyItemService(IDataContext db, INWScript script)
        {
            _db = db;
            _ = script;
        }

        public bool PlayerHasKeyItem(NWObject oPC, int keyItemID)
        {
            PCKeyItem entity = _db.PCKeyItems.FirstOrDefault(x => x.PlayerID == oPC.GlobalID && x.KeyItemID == keyItemID);
            return entity != null;
        }


        public void GivePlayerKeyItem(NWPlayer oPC, int keyItemID)
        {
            if (!PlayerHasKeyItem(oPC, keyItemID))
            {
                PCKeyItem entity = new PCKeyItem
                {
                    PlayerID = oPC.GlobalID,
                    KeyItemID = keyItemID
                };
                _db.PCKeyItems.Add(entity);
                _db.SaveChanges();

                KeyItem keyItem = _db.KeyItems.Single(x => x.KeyItemID == keyItemID);
                oPC.SendMessage("You acquired the key item '" + keyItem.Name + "'.");
            }
        }

        public void RemovePlayerKeyItem(NWPlayer oPC, int keyItemID)
        {
            if (PlayerHasKeyItem(oPC, keyItemID))
            {

                PCKeyItem entity = _db.PCKeyItems.Single(x => x.PlayerID == oPC.GlobalID && x.KeyItemID == keyItemID);
                _db.PCKeyItems.Remove(entity);
                _db.SaveChanges();
            }
        }

        public IEnumerable<PCKeyItem> GetPlayerKeyItemsByCategory(NWPlayer player, int categoryID)
        {
            return _db.PCKeyItems.Where(x => x.PlayerID == player.GlobalID && x.KeyItem.KeyItemCategoryID == categoryID).ToList();
        }

        public KeyItem GetKeyItemByID(int keyItemID)
        {
            return _db.KeyItems.Single(x => x.KeyItemID == keyItemID);
        }

        public void OnModuleItemAcquired()
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetModuleItemAcquiredBy());

            if (!oPC.IsPlayer) return;

            NWItem oItem = NWItem.Wrap(_.GetModuleItemAcquired());
            int keyItemID = oItem.GetLocalInt("KEY_ITEM_ID");

            if (keyItemID <= 0) return;

            GivePlayerKeyItem(oPC, keyItemID);
            oItem.Destroy();
        }
    }
}
