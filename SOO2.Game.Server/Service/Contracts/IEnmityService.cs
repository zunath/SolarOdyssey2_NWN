using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.ValueObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IEnmityService
    {
        void AdjustEnmity(NWCreature npc, NWCreature attacker, int volatileAdjust, int cumulativeAdjust = 0);
        void AdjustEnmityOnAllTaggedCreatures(NWCreature attacker, int volatileAdjust, int cumulativeAdjust = 0);
        EnmityTable GetEnmityTable(NWCreature npc);
        void OnNPCDamaged();
        void OnNPCPhysicallyAttacked();
        void OnPlayerDamaged();
        bool IsOnEnmityTable(NWCreature npc, NWCreature target);
        bool IsEnmityTableEmpty(NWCreature npc);
    }
}