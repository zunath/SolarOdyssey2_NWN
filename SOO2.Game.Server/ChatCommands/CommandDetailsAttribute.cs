using System;
using SOO2.Game.Server.Enumeration;

namespace SOO2.Game.Server.ChatCommands
{
    public class CommandDetailsAttribute: Attribute
    {
        public string Description { get; set; }
        public CommandPermissionType Permissions { get; set; }

        public CommandDetailsAttribute(string description, CommandPermissionType permissions)
        {
            Description = description;
            Permissions = permissions;
        }

    }
}
