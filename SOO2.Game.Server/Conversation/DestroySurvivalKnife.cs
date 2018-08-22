﻿using NWN;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using SOO2.Game.Server.ValueObject.Dialog;

namespace SOO2.Game.Server.Conversation
{
    internal class DestroySurvivalKnife: ConversationBase
    {
        public DestroySurvivalKnife(INWScript script, IDialogService dialog) 
            : base(script, dialog)
        {
        }

        public override PlayerDialog SetUp(NWPlayer player)
        {
            PlayerDialog dialog = new PlayerDialog("MainPage");
            DialogPage mainPage = new DialogPage(
                "Are you sure you want to destroy your survival knife? This action is irreversible!",
                "Destroy Survival Knife"
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
                    NWItem item = NWItem.Wrap(_.GetItemPossessedBy(player.Object, "survival_knife"));
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
