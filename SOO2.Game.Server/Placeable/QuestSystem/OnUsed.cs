﻿using NWN;
using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Placeable.QuestSystem
{
    public class OnUsed: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IQuestService _quest;

        public OnUsed(IQuestService quest)
        {
            _quest = quest;
        }

        public bool Run(params object[] args)
        {
            NWPlaceable container = NWPlaceable.Wrap(_.GetLastUsedBy());
            _quest.OnQuestPlaceableUsed(container);
            return true;
        }
    }
}
