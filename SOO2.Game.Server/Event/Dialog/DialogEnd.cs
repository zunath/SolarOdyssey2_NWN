﻿using NWN;
using SOO2.Game.Server.Conversation.Contracts;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using SOO2.Game.Server.ValueObject.Dialog;

namespace SOO2.Game.Server.Event.Dialog
{
    public class DialogEnd: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IDialogService _dialog;

        public DialogEnd(INWScript script, IDialogService dialog)
        {
            _ = script;
            _dialog = dialog;
        }

        public bool Run(params object[] args)
        {
            NWPlayer player = NWPlayer.Wrap(_.GetPCSpeaker());
            PlayerDialog dialog = _dialog.LoadPlayerDialog(player.GlobalID);
            
            IConversation convo = App.ResolveByInterface<IConversation>("Conversation." + dialog.ActiveDialogName);
            convo.EndDialog();
            _dialog.RemovePlayerDialog(player.GlobalID);
            player.DeleteLocalInt("DIALOG_SYSTEM_INITIALIZE_RAN");

            return true;
        }
    }
}
