using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Event.Module
{
    internal class OnModuleRespawn : IRegisteredEvent
    {
        private readonly IDeathService _death;

        public OnModuleRespawn(IDeathService death)
        {
            _death = death;
        }

        public bool Run(params object[] args)
        {
            _death.OnPlayerRespawn();
            return true;
        }
    }
}
