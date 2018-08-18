using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.ChatCommand.Contracts
{
    public interface IChatCommand
    {
        void DoAction(NWPlayer user, params string[] args);
    }
}
