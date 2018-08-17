using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.ValueObject.Dialog;

namespace SOO2.Game.Server.Conversation.Contracts
{
    internal interface IConversation
    {
        PlayerDialog SetUp(NWPlayer player);
        void Initialize();
        void DoAction(NWPlayer player, string pageName, int responseID);
        void EndDialog();
    }
}
