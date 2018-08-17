using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IPVPSanctuaryService
    {
        bool PlayerHasPVPSanctuary(NWPlayer player);
        void SetPlayerPVPSanctuaryOverride(NWPlayer player, bool overrideStatus);
        bool IsPVPAttackAllowed(NWPlayer attacker, NWPlayer target);
    }
}
