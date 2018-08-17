using System;
using System.Collections.Generic;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using SOO2.Game.Server.ValueObject;

namespace SOO2.Game.Server.Service
{
    public class EnmityService : IEnmityService
    {
        private readonly AppState _state;

        public EnmityService(AppState state)
        {
            _state = state;
        }

        public void AdjustEnmity(NWCreature npc, NWCreature attacker, int adjustBy)
        {
            var table = GetEnmityTable(npc);
            if (!table.ContainsKey(attacker.GlobalID))
            {
                table.Add(attacker.GlobalID, new Enmity(attacker));
            }

            table[attacker.GlobalID].Amount += adjustBy;

            if (table[attacker.GlobalID].Amount < 0)
                table[attacker.GlobalID].Amount = 0;
        }
        
        public Dictionary<string, Enmity> GetEnmityTable(NWCreature npc)
        {
            if (!npc.IsNPC) throw new Exception("Only NPCs have enmity tables.");

            if (!npc.Data.ContainsKey("EnmityTable"))
            {
                npc.Data["EnmityTable"] = new Dictionary<string, Enmity>();
            }

            return npc.Data["EnmityTable"];
        }
    }
}
