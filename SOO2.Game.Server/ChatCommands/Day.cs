using Freescape.Game.Server.ChatCommands.Contracts;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using NWN;

namespace Freescape.Game.Server.ChatCommands
{
    [CommandDetails("Sets the world time to 8 AM.", CommandPermissionType.DM)]
    public class Day: IChatCommand
    {
        private readonly INWScript _;
        
        public Day(INWScript script)
        {
            _ = script;
        }
        
        public void DoAction(NWPlayer user, params string[] args)
        {
            _.SetTime(8, 0, 0, 0);
        }
    }
}
