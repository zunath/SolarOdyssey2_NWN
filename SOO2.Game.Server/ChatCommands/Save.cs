using SOO2.Game.Server.ChatCommands.Contracts;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject;
using NWN;

namespace SOO2.Game.Server.ChatCommands
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
