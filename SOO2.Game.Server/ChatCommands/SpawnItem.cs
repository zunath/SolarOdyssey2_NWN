﻿using Freescape.Game.Server.ChatCommands.Contracts;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.ChatCommands
{
    [CommandDetails("Spawns an item of a specific quantity on your character. Example: /spawnitem my_item 3", CommandPermissionType.DM)]
    public class SpawnItem: IChatCommand
    {
        private readonly INWScript _;
        private readonly IColorTokenService _color;

        public SpawnItem(
            INWScript script,
            IColorTokenService color)
        {
            _ = script;
            _color = color;
        }
        
        public void DoAction(NWPlayer user, params string[] args)
        {
            if (args.Length <= 0)
            {
                user.SendMessage(_color.Red("Please specify a resref and optionally a quantity. Example: /" + nameof(SpawnItem) + " my_resref 20"));
                return;
            }
            string resref = args[0];
            int quantity = 1;

            if (args.Length > 1)
            {
                if (!int.TryParse(args[1], out quantity))
                {
                    return;
                }
            }

            var item = NWItem.Wrap(_.CreateItemOnObject(resref, user.Object, quantity));

            if (!item.IsValid)
            {
                user.SendMessage(_color.Red("Item not found! Did you enter the correct ResRef?"));
                return;
            }

            item.IsIdentified = true;
        }
    }
}
