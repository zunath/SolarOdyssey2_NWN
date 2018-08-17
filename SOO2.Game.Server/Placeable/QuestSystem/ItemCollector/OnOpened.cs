using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.Placeable.QuestSystem.ItemCollector
{
    public class OnOpened : IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IQuestService _quest;

        public OnOpened(IQuestService quest)
        {
            _quest = quest;
        }

        public bool Run(params object[] args)
        {
            NWPlaceable container = NWPlaceable.Wrap(Object.OBJECT_SELF);
            _quest.OnItemCollectorOpened(container);
            return true;
        }
    }
}
