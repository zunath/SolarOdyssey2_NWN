using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.ValueObject.Dialog;

namespace Freescape.Game.Server.Conversation.Contracts
{
    internal interface IConversation
    {
        PlayerDialog SetUp(NWPlayer player);
        void Initialize();
        void DoAction(NWPlayer player, string pageName, int responseID);
        void EndDialog();
    }
}
