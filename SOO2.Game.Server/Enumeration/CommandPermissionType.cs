using System;

namespace SOO2.Game.Server.Enumeration
{
    [Flags]
    public enum CommandPermissionType
    {
        Player = 1,
        DM = 2
    }
}
