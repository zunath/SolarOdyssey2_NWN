﻿using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.Placeable.GrowingPlant
{
    public class OnUsed: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IFarmingService _farming;

        public OnUsed(INWScript script, IFarmingService farming)
        {
            _ = script;
            _farming = farming;
        }

        public bool Run(params object[] args)
        {
            NWPlaceable plant = NWPlaceable.Wrap(Object.OBJECT_SELF);
            int growingPlantID = plant.GetLocalInt("GROWING_PLANT_ID");
            if (growingPlantID <= 0) return false;

            NWPlayer oPC = NWPlayer.Wrap(_.GetLastUsedBy());
            Data.Entities.GrowingPlant growingPlant = _farming.GetGrowingPlantByID(growingPlantID);
            if (growingPlant.WaterStatus <= 0)
            {
                oPC.SendMessage("This plant doesn't seem to need anything right now.");
                return true;
            }

            oPC.SendMessage("This plant needs to be watered. Use a Water Jug on it to water it. These can be crafted with the Metalworking skill.");
            return true;
        }
    }
}
