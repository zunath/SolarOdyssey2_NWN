using SOO2.Game.Server.ChatCommands.Contracts;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject;
using NWN;

namespace SOO2.Game.Server.ChatCommands
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
