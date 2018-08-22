using NWN;
using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Placeable.DeathCorpse
{
    public class OnDisturbed: IRegisteredEvent
    {
        private readonly IDeathService _death;

        public OnDisturbed(IDeathService death)
        {
            _death = death;
        }

        public bool Run(params object[] args)
        {
            _death.OnCorpseDisturb(NWPlaceable.Wrap(Object.OBJECT_SELF));
            return true;
        }
    }
}
