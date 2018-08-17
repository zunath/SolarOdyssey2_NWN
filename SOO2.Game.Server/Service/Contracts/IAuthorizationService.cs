using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IAuthorizationService
    {
        bool IsPCRegisteredAsDM(NWPlayer player);
    }
}
