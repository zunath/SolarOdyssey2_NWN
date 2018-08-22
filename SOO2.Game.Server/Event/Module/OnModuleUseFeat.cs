using NWN;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Event.Module
{
    internal class OnModuleUseFeat : IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IAbilityService _ability;
        
        public OnModuleUseFeat(
            INWScript script,
            IAbilityService ability)
        {
            _ = script;
            _ability = ability;
        }

        public bool Run(params object[] args)
        {
            _ability.OnModuleUseFeat();
            return true;

        }
    }
}
