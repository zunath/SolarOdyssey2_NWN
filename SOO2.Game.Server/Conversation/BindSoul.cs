using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using Freescape.Game.Server.ValueObject.Dialog;
using NWN;

namespace Freescape.Game.Server.Conversation
{
    public class BindSoul: ConversationBase
    {
        private readonly IDeathService _death;

        public BindSoul(
            INWScript script, 
            IDialogService dialog,
            IDeathService death) 
            : base(script, dialog)
        {
            _death = death;
        }

        public override PlayerDialog SetUp(NWPlayer player)
        {
            PlayerDialog dialog = new PlayerDialog("MainPage");
            DialogPage mainPage = new DialogPage(
                "If you die, you will respawn at the last place you bound your soul. Would you like to bind your soul to this location?",
                "Bind my soul"
            );

            dialog.AddPage("MainPage", mainPage);
            return dialog;
        }

        public override void Initialize()
        {
        }

        public override void DoAction(NWPlayer player, string pageName, int responseID)
        {
            if (responseID == 1)
            {
                _death.BindPlayerSoul(player, true);
                EndConversation();
            }
        }

        public override void EndDialog()
        {
        }
    }
}
