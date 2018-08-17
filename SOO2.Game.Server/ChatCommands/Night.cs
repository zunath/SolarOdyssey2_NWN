using Freescape.Game.Server.ChatCommands.Contracts;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using NWN;

namespace Freescape.Game.Server.ChatCommands
{
    [CommandDetails("Sets the world time to 8 PM.", CommandPermissionType.DM)]
    public class Night : IChatCommand
    {
        private readonly INWScript _;
        
        public Night(INWScript script)
        {
            _ = script;
        }
        
        public void DoAction(NWPlayer user, params string[] args)
        {
            _.SetTime(20, 0, 0, 0);
        }
    }
}
