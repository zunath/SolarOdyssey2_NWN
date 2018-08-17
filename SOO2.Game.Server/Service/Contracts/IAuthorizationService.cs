using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface IAuthorizationService
    {
        bool IsPCRegisteredAsDM(NWPlayer player);
    }
}
