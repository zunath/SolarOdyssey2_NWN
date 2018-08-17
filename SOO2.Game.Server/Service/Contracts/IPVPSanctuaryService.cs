using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface IPVPSanctuaryService
    {
        bool PlayerHasPVPSanctuary(NWPlayer player);
        void SetPlayerPVPSanctuaryOverride(NWPlayer player, bool overrideStatus);
        bool IsPVPAttackAllowed(NWPlayer attacker, NWPlayer target);
    }
}
