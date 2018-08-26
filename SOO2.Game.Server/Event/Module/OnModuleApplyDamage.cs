using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Event.Module
{
    public class OnModuleApplyDamage: IRegisteredEvent
    {
        private readonly IRuneService _rune;

        public OnModuleApplyDamage(IRuneService rune)
        {
            _rune = rune;
        }

        public bool Run(params object[] args)
        {
            _rune.OnModuleApplyDamage();
            return true;
        }
    }
}
