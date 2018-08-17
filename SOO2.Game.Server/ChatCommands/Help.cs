﻿using System;
using System.Linq;
using System.Reflection;
using Freescape.Game.Server.ChatCommands.Contracts;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;

namespace Freescape.Game.Server.ChatCommands
{
    [CommandDetails("Displays all chat commands available to you.", CommandPermissionType.DM | CommandPermissionType.Player)]
    public class Help: IChatCommand
    {
        private readonly IColorTokenService _color;
        private readonly IAuthorizationService _auth;

        public Help(IColorTokenService color,
            IAuthorizationService auth)
        {
            _color = color;
            _auth = auth;
        }
        
        public void DoAction(NWPlayer user, params string[] args)
        {
            bool isDM = user.IsDM || _auth.IsPCRegisteredAsDM(user);

            var classes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IChatCommand).IsAssignableFrom(p) && p.IsClass)
                .OrderBy(o => o.Name)
                .ToArray();

            foreach (var @class in classes)
            {
                var attribute = @class.GetCustomAttribute<CommandDetailsAttribute>();
                if (attribute == null) continue;
                
                if (attribute.Permissions.HasFlag(CommandPermissionType.Player) && user.IsPlayer ||
                    attribute.Permissions.HasFlag(CommandPermissionType.DM) && isDM)
                {
                    user.SendMessage(_color.Green("/" + @class.Name.ToLower()) + _color.White(": " + attribute.Description));
                }
            }
        }
    }
}
