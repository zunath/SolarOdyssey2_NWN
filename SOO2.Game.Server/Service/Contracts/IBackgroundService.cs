using System.Collections.Generic;
using SOO2.Game.Server.Data.Entities;
using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IBackgroundService
    {
        IEnumerable<Background> GetActiveBackgrounds();
        void SetPlayerBackground(NWPlayer player, Background background);
        void OnModuleClientEnter();
    }
}
