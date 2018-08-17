using System.Collections.Generic;

namespace SOO2.Game.Server.GameObject.Contracts
{
    public interface INWModule
    {
        List<NWPlayer> Players { get; }
    }
}