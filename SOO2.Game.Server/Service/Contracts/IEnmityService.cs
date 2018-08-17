using System.Collections.Generic;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.ValueObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IEnmityService
    {
        void AdjustEnmity(NWCreature npc, NWCreature attacker, int adjustBy);
        Dictionary<string, Enmity> GetEnmityTable(NWCreature npc);
    }
}