﻿using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using Freescape.Game.Server.ValueObject.Dialog;
using NWN;

namespace Freescape.Game.Server.Conversation
{
    internal class DestroyPlayerGuide: ConversationBase
    {
        public DestroyPlayerGuide(INWScript script, IDialogService dialog) 
            : base(script, dialog)
        {
        }

        public override PlayerDialog SetUp(NWPlayer player)
        {
            PlayerDialog dialog = new PlayerDialog("MainPage");
            DialogPage mainPage = new DialogPage(
                "Are you sure you want to destroy your player guide? This action is irreversible!",
                "Destroy Player Guide"
            );

            dialog.AddPage("MainPage", mainPage);
            return dialog;
        }

        public override void Initialize()
        {
        }

        public override void DoAction(NWPlayer player, string pageName, int responseID)
        {
            switch (responseID)
            {
                case 1:
                    NWItem item = NWItem.Wrap(_.GetItemPossessedBy(player.Object, "player_guide"));
                    _.DestroyObject(item.Object);
                    EndConversation();
                    break;
            }
        }

        public override void EndDialog()
        {
        }
    }
}
