using System;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using NWN;

namespace SOO2.Game.Server.Event.Player
{
    public class OnPlayerDamaged : IRegisteredEvent
    {
        private readonly IEnmityService _enmity;

        public OnPlayerDamaged(IEnmityService enmity)
        {
            _enmity = enmity;
        }

        public bool Run(params object[] args)
        {
            _enmity.OnPlayerDamaged();
            return true;
        }
    }
}
