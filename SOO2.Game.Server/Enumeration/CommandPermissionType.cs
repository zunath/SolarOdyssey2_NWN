using System;

namespace Freescape.Game.Server.Enumeration
{
    [Flags]
    public enum CommandPermissionType
    {
        Player = 1,
        DM = 2
    }
}
