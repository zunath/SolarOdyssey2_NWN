using System.Collections.Generic;

namespace Freescape.Game.Server.GameObject.Contracts
{
    public interface INWModule
    {
        List<NWPlayer> Players { get; }
    }
}