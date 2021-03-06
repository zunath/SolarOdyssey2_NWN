﻿using NWN;
using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Placeable.Resource
{
    public class OnUsed: IRegisteredEvent
    {
        private readonly INWScript _;

        public OnUsed(INWScript script)
        {
            _ = script;
        }

        public bool Run(params object[] args)
        {
            Object self = Object.OBJECT_SELF;
            NWObject oPC = NWObject.Wrap(_.GetLastUsedBy());
            oPC.AssignCommand(() => _.ActionAttack(self));
            return true;
        }
    }
}
