﻿using System.Linq;
using Freescape.Game.Server.ChatCommands.Contracts;
using Freescape.Game.Server.Data.Contracts;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using NWN;

namespace Freescape.Game.Server.ChatCommands
{
    [CommandDetails("ALPHA Command: Returns you to your bind point.", CommandPermissionType.Player)]
    public class Stuck: IChatCommand
    {
        private readonly INWScript _;
        private readonly IDataContext _db;

        public Stuck(INWScript script, IDataContext db)
        {
            _ = script;
            _db = db;
        }
        
        public void DoAction(NWPlayer user, params string[] args)
        {
            PlayerCharacter pc = _db.PlayerCharacters.Single(x => x.PlayerID == user.GlobalID);
            Location location = _.Location(
                _.GetObjectByTag(pc.RespawnAreaTag),
                _.Vector((float)pc.RespawnLocationX, (float)pc.RespawnLocationY, (float)pc.RespawnLocationZ),
                (float)pc.RespawnLocationOrientation
            );
            user.AssignCommand(() => _.ActionJumpToLocation(location));
            _.SendMessageToPC(user.Object, "Alpha feature: Returning to bind point. Please report bugs on Discord/GitHub. And for the love of all that is Zunath, don't abuse this!");
        }
    }
}
