﻿using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Placeable.ForagePoint
{
    public class OnDisturbed: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IFarmingService _farming;
        private readonly IRandomService _random;
        private readonly IItemService _item;
        private readonly IPerkService _perk;

        public OnDisturbed(
            INWScript script,
            IFarmingService farming,
            IRandomService random,
            IItemService item,
            IPerkService perk)
        {
            _ = script;
            _farming = farming;
            _random = random;
            _item = item;
            _perk = perk;
        }

        public bool Run(params object[] args)
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetLastDisturbed());
            NWItem oItem = NWItem.Wrap(_.GetInventoryDisturbItem());
            NWPlaceable point = NWPlaceable.Wrap(NWN.Object.OBJECT_SELF);
            int disturbType = _.GetInventoryDisturbType();

            if (disturbType == INVENTORY_DISTURB_TYPE_ADDED)
            {
                _item.ReturnItem(oPC, oItem);
            }
            else
            {
                if (point.InventoryItems.Count <= 0 && point.GetLocalInt("FORAGE_POINT_FULLY_HARVESTED") == 1)
                {
                    string seed = point.GetLocalString("FORAGE_POINT_SEED");
                    if (!string.IsNullOrWhiteSpace(seed))
                    {
                        _.CreateObject(OBJECT_TYPE_ITEM, seed, point.Location);

                        int perkLevel = _perk.GetPCPerkLevel(oPC, PerkType.SeedPicker);
                        if (_random.Random(100) + 1 <= perkLevel * 10)
                        {
                            _.CreateObject(OBJECT_TYPE_ITEM, seed, point.Location);
                        }
                    }

                    point.Destroy();
                    _farming.RemoveGrowingPlant(point);
                }
            }
            return true;
        }
    }
}
