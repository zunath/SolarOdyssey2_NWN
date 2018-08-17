using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Event.Module
{
    internal class OnModuleRest : IRegisteredEvent
    {
        private readonly INWScript _nw;
        private readonly IDialogService _dialog;

        public OnModuleRest(INWScript nw, IDialogService dialog)
        {
            _nw = nw;
            _dialog = dialog;
        }

        public bool Run(params object[] args)
        {
            NWPlayer player = NWPlayer.Wrap(_nw.GetLastPCRested());
            int restType = _nw.GetLastRestEventType();

            if (restType != REST_EVENTTYPE_REST_STARTED ||
                !player.IsValid ||
                player.IsDM) return false;

            player.AssignCommand(() => _nw.ClearAllActions());

            _dialog.StartConversation(player, player, "RestMenu");
            
            return true;
        }
    }
}
