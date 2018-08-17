using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Event.Module
{
    internal class OnModuleChat : IRegisteredEvent
    {
        private readonly IPlayerDescriptionService _playerDescription;

        public OnModuleChat(
            IPlayerDescriptionService playerDescription)
        {
            _playerDescription = playerDescription;
        }

        public bool Run(params object[] args)
        {
            _playerDescription.OnModuleChat();

            return true;

        }
    }
}
