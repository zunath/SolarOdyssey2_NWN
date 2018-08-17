using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface ILocalVariableService
    {
        void CopyVariables(NWObject source, NWObject copy);
    }
}
