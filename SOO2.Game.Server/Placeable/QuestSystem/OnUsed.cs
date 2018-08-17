using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.Placeable.QuestSystem
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
