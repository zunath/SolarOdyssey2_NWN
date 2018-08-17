using Freescape.Game.Server.Conversation.Contracts;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using Freescape.Game.Server.ValueObject.Dialog;
using NWN;

namespace Freescape.Game.Server.Event.Dialog
{
    public class ActionTaken: IRegisteredEvent
    {
        private readonly IDialogService _dialogService;
        private readonly INWScript _;

        public ActionTaken(INWScript script, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _ = script;
        }

        public bool Run(params object[] args)
        {
            int nodeID = (int)args[0];
            NWPlayer player = NWPlayer.Wrap(_.GetPCSpeaker());
            PlayerDialog dialog = _dialogService.LoadPlayerDialog(player.GlobalID);
            int selectionNumber = nodeID + 1;
            int responseID = nodeID + (_dialogService.NumberOfResponsesPerPage * dialog.PageOffset);

            if (selectionNumber == _dialogService.NumberOfResponsesPerPage + 1) // Next page
            {
                dialog.PageOffset = dialog.PageOffset + 1;
            }
            else if (selectionNumber == _dialogService.NumberOfResponsesPerPage + 2) // Previous page
            {
                dialog.PageOffset = dialog.PageOffset - 1;
            }
            else if (selectionNumber != _dialogService.NumberOfResponsesPerPage + 3) // End
            {
                IConversation convo = App.ResolveByInterface<IConversation>("Conversation." + dialog.ActiveDialogName);
                convo.DoAction(player, dialog.CurrentPageName, responseID + 1);
            }

            return true;
        }
    }
}
