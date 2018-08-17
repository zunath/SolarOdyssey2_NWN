using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface ILocalVariableService
    {
        void CopyVariables(NWObject source, NWObject copy);
    }
}
