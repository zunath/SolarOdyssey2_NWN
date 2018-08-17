using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.ChatCommands.Contracts
{
    public interface IChatCommand
    {
        void DoAction(NWPlayer user, params string[] args);
    }
}
