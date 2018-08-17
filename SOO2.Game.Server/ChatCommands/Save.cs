using Freescape.Game.Server.ChatCommands.Contracts;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using NWN;

namespace Freescape.Game.Server.ChatCommands
{
    [CommandDetails("Saves your character.", CommandPermissionType.Player)]
    public class Save: IChatCommand
    {
        private readonly INWScript _;

        public Save(INWScript script)
        {
            _ = script;
        }

        public void DoAction(NWPlayer user, params string[] args)
        {
            _.ExportSingleCharacter(user.Object);
            _.SendMessageToPC(user.Object, "Character saved successfully.");
        }
    }
}
