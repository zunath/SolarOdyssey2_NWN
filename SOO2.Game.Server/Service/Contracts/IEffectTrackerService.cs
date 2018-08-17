using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IEffectTrackerService
    {
        void ProcessPCEffects(NWPlayer oPC);
    }
}