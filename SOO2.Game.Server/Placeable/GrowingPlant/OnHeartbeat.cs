﻿using System.Linq;
using NWN;
using SOO2.Game.Server.Data.Contracts;
using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using static NWN.NWScript;

namespace SOO2.Game.Server.Placeable.GrowingPlant
{
    public class OnHeartbeat: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IDataContext _db;

        public OnHeartbeat(INWScript script,
            IDataContext db)
        {
            _ = script;
            _db = db;
        }

        public bool Run(params object[] args)
        {
            NWPlaceable plant = NWPlaceable.Wrap(Object.OBJECT_SELF);
            int growingPlantID = plant.GetLocalInt("GROWING_PLANT_ID");
            if (growingPlantID <= 0) return false;
            
            Data.Entities.GrowingPlant growingPlant = _db.GrowingPlants.Single(x => x.GrowingPlantID == growingPlantID);
            growingPlant.RemainingTicks--;
            growingPlant.TotalTicks++;

            int waterTicks = growingPlant.Plant.WaterTicks;
            if (waterTicks > 0 && growingPlant.TotalTicks % waterTicks == 0)
            {
                int maxWaterStatus = growingPlant.Plant.BaseTicks / growingPlant.Plant.WaterTicks;

                if (growingPlant.WaterStatus < maxWaterStatus)
                {
                    growingPlant.WaterStatus++;
                    growingPlant.RemainingTicks = growingPlant.RemainingTicks * growingPlant.WaterStatus;
                }
            }

            if (growingPlant.RemainingTicks <= 0)
            {
                plant.Destroy();
                plant = NWPlaceable.Wrap(_.CreateObject(OBJECT_TYPE_PLACEABLE, growingPlant.Plant.Resref, plant.Location));
                plant.SetLocalInt("GROWING_PLANT_ID", growingPlantID);
            }
            
            _db.SaveChanges();
            return true;
        }
    }
}
