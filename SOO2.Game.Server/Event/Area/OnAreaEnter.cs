using Freescape.Game.Server.Service.Contracts;

namespace Freescape.Game.Server.Event.Area
{
    internal class OnAreaEnter: IRegisteredEvent
    {
        private readonly IMigrationService _migration;
        private readonly IPlayerService _player;

        public OnAreaEnter(
            IMigrationService migration,
            IPlayerService player)
        {
            _migration = migration;
            _player = player;
        }

        public bool Run(params object[] args)
        {
            _migration.OnAreaEnter();
            _player.OnAreaEnter();

            return true;
        }
    }
}
