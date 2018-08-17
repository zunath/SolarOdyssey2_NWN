using Freescape.Game.Server.Service.Contracts;

namespace Freescape.Game.Server.Event.Module
{
    internal class OnModuleDying : IRegisteredEvent
    {
        private readonly IDeathService _death;

        public OnModuleDying(IDeathService death)
        {
            _death = death;
        }

        public bool Run(params object[] args)
        {
            _death.OnPlayerDying();

            return true;

        }
    }
}
