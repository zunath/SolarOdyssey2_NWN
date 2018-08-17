using System;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Service
{
    public class PlayerDescriptionService: IPlayerDescriptionService
    {
        private readonly INWScript _;

        public PlayerDescriptionService(INWScript script)
        {
            _ = script;
        }

        public void OnModuleChat()
        {
            NWPlayer sender = NWPlayer.Wrap(_.GetPCChatSpeaker());
            if (sender.GetLocalInt("LISTENING_FOR_DESCRIPTION") != 1) return;
            if (!sender.IsPlayer) return;

            string text = _.GetPCChatMessage().Trim();
            sender.SetLocalString("NEW_DESCRIPTION_TO_SET", text);

            _.SetPCChatMessage(string.Empty); // Skip the message

            _.SendMessageToPC(sender.Object, "New description received. Please press the 'Next' button in the conversation window.");
        }

        public void ChangePlayerDescription(NWPlayer player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));
            if (player.Object == null) throw new ArgumentNullException(nameof(player.Object));

            string newDescription = player.GetLocalString("NEW_DESCRIPTION_TO_SET");
            _.SetDescription(player.Object, newDescription, FALSE);
            _.SetDescription(player.Object, newDescription);

            _.FloatingTextStringOnCreature("New description set!", player.Object, FALSE);
        }
    }
}
