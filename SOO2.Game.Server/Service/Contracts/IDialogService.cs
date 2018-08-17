using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.ValueObject.Dialog;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IDialogService
    {
        int NumberOfResponsesPerPage { get; }
        PlayerDialog LoadPlayerDialog(string globalID);
        void RemovePlayerDialog(string globalID);
        void LoadConversation(NWPlayer player, NWObject talkTo, string @class, int dialogNumber);
        void StartConversation(NWPlayer player, NWObject talkTo, string @class);
        void StartConversation(NWCreature player, NWObject talkTo, string @class);
        void EndConversation(NWPlayer player);
    }
}
