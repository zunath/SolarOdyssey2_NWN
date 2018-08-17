﻿using System.Collections.Generic;
using SOO2.Game.Server.GameObject.Contracts;
using NWN;

namespace SOO2.Game.Server.GameObject
{
    public class NWModule: NWObject, INWModule
    {
        public NWModule(INWScript script,
            AppState state) 
            : base(script, state)
        {
        }

        public static NWModule Get()
        {
            var module = (NWModule) App.Resolve<INWModule>();
            INWScript _ = App.Resolve<INWScript>();
            module.Object = _.GetModule();

            return module;
        }

        public List<NWPlayer> Players
        {
            get
            {
                List<NWPlayer> players = new List<NWPlayer>();

                NWPlayer player = NWPlayer.Wrap(_.GetFirstPC());
                while (player.IsValid)
                {
                    players.Add(player);
                    player = NWPlayer.Wrap(_.GetNextPC());
                }

                return players;
            }
        }

    }
}
