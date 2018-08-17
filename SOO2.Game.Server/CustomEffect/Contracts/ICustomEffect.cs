using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.CustomEffect.Contracts
{
    public interface ICustomEffect
    {
        void Apply(NWCreature oCaster, NWObject oTarget);
        void Tick(NWCreature oCaster, NWObject oTarget);
        void WearOff(NWCreature oCaster, NWObject oTarget);
    }
}
