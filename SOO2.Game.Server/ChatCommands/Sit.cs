using Freescape.Game.Server.ChatCommands.Contracts;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using NWN;

namespace Freescape.Game.Server.ChatCommands
{
    [CommandDetails("Makes your character sit down.", CommandPermissionType.DM | CommandPermissionType.Player)]
    public class Sit: IChatCommand
    {
        private readonly INWScript _;

        public Sit(INWScript script)
        {
            _ = script;
        }

        public void DoAction(NWPlayer user, params string[] args)
        {
            user.AssignCommand(() => _.ActionPlayAnimation(NWScript.ANIMATION_LOOPING_SIT_CROSS, 1.0f, 9999));
        }
    }
}
