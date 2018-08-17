﻿using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface IDeathService
    {
        void BindPlayerSoul(NWPlayer player, bool showMessage);
        void OnCorpseClose(NWPlaceable corpse);
        void OnCorpseDisturb(NWPlaceable corpse);
        void OnModuleLoad();
        void OnPlayerDeath();
        void OnPlayerDying();
        void OnPlayerRespawn();
        void TeleportPlayerToBindPoint(NWPlayer pc);
    }
}
