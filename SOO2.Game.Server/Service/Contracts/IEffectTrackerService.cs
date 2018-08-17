using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface IEffectTrackerService
    {
        void ProcessPCEffects(NWPlayer oPC);
    }
}