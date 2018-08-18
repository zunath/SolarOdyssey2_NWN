using NWN;
using SOO2.Game.Server.ChatCommand.Contracts;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.ChatCommand
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
