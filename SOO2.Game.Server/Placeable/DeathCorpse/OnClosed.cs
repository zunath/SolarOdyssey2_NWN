using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.Placeable.DeathCorpse
{
    public class OnClosed: IRegisteredEvent
    {
        private readonly IDeathService _death;

        public OnClosed(IDeathService death)
        {
            _death = death;
        }

        public bool Run(params object[] args)
        {
            _death.OnCorpseClose(NWPlaceable.Wrap(Object.OBJECT_SELF));
            return true;
        }
    }
}
