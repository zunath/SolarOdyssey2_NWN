using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.ValueObject.Dialog;

namespace Freescape.Game.Server.Service.Contracts
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
