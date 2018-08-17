﻿using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.Event.Module
{
    internal class OnModuleAcquireItem: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IKeyItemService _keyItem;
        private readonly IQuestService _quest;
        private readonly IItemService _item;

        public OnModuleAcquireItem(
            INWScript script, 
            IKeyItemService keyItem,
            IQuestService quest,
            IItemService item)
        {
            _ = script;
            _keyItem = keyItem;
            _quest = quest;
            _item = item;
        }

        public bool Run(params object[] args)
        {
            // Bioware Default
            _.ExecuteScript("x2_mod_def_aqu", Object.OBJECT_SELF);
            _keyItem.OnModuleItemAcquired();
            _quest.OnModuleItemAcquired();
            _item.OnModuleItemAcquired();
            return true;

        }
    }
}
