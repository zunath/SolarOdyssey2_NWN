﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NWN;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;
using SOO2.Game.Server.ValueObject;
using Object = NWN.Object;

namespace SOO2.Game.Server.Service
{
    public class EnmityService : IEnmityService
    {
        private readonly INWScript _;
        private readonly AppState _state;
        private readonly IObjectProcessingService _ops;

        public EnmityService(INWScript script,
            AppState state,
            IObjectProcessingService ops)
        {
            _ = script;
            _state = state;
            _ops = ops;
        }

        private Enmity GetEnmity(NWCreature npc, NWCreature attacker)
        {
            var table = GetEnmityTable(npc);
            if (!table.ContainsKey(attacker.GlobalID))
            {
                table.Add(attacker.GlobalID, new Enmity(attacker));
            }

            return table[attacker.GlobalID];
        }

        private Dictionary<string, EnmityTable> GetAllNPCEnmityTablesForCreature(NWCreature player)
        {
            if (!player.IsPlayer) throw new Exception(nameof(GetAllNPCEnmityTablesForCreature) + " can only be used with players.");

            var npcTables = new Dictionary<string, EnmityTable>();

            foreach (var npcTable in _state.NPCEnmityTables)
            {
                if (npcTable.Value.ContainsKey(player.GlobalID))
                {
                    npcTables.Add(npcTable.Key, npcTable.Value);
                }
            }

            return npcTables;
        }
        
        public void AdjustEnmity(NWCreature npc, NWCreature attacker, int volatileAdjust, int cumulativeAdjust = 0)
        {
            bool adjustVolatile = volatileAdjust != 0;
            bool adjustCumulative = cumulativeAdjust != 0;

            float effectiveEnmityRate = 1.0f;
            if (attacker.IsPlayer)
            {
                NWPlayer player = NWPlayer.Wrap(attacker.Object);
                effectiveEnmityRate = player.EffectiveEnmityRate;
            }

            volatileAdjust = (int)(effectiveEnmityRate * volatileAdjust);
            cumulativeAdjust = (int) (effectiveEnmityRate * cumulativeAdjust);

            var table = GetEnmity(npc, attacker);

            if (adjustVolatile)
            {
                table.VolatileAmount += volatileAdjust;
            }
            if (adjustCumulative)
            {
                table.CumulativeAmount += cumulativeAdjust;
            }
        }
        
        public void AdjustEnmityOnAllTaggedCreatures(NWCreature attacker, int volatileAdjust, int cumulativeAdjust = 0)
        {
            var tables = GetAllNPCEnmityTablesForCreature(attacker);
            foreach (var table in tables)
            {
                AdjustEnmity(table.Value.NPCObject, attacker, volatileAdjust, cumulativeAdjust);
            }
        }

        public void OnNPCPhysicallyAttacked()
        {
            NWCreature self = NWCreature.Wrap(Object.OBJECT_SELF);
            NWCreature attacker = NWCreature.Wrap(_.GetLastAttacker(Object.OBJECT_SELF));
            AdjustEnmity(self, attacker, 0, 1);
        }

        public void OnNPCDamaged()
        {
            NWCreature self = NWCreature.Wrap(Object.OBJECT_SELF);
            NWCreature damager = NWCreature.Wrap(_.GetLastDamager(Object.OBJECT_SELF));
            int enmityAmount = _.GetTotalDamageDealt();
            if (enmityAmount <= 0) enmityAmount = 1;
            
            AdjustEnmity(self, damager, 0, enmityAmount);
        }

        public void OnPlayerDamaged()
        {
            NWPlayer player = NWPlayer.Wrap(Object.OBJECT_SELF);
            NWCreature npc = NWCreature.Wrap(_.GetLastDamager(Object.OBJECT_SELF));

            if (!player.IsPlayer || !npc.IsNPC) return;

            int damage = _.GetTotalDamageDealt();
            Enmity enmity = GetEnmity(npc, player);
            enmity.CumulativeAmount -= damage;
        }

        public EnmityTable GetEnmityTable(NWCreature npc)
        {
            if (!npc.IsNPC) throw new Exception("Only NPCs have enmity tables.");
            
            if (!_state.NPCEnmityTables.ContainsKey(npc.GlobalID))
            {
                _state.NPCEnmityTables.Add(npc.GlobalID, new EnmityTable(npc));
            }

            return _state.NPCEnmityTables[npc.GlobalID];
        }

        public bool IsOnEnmityTable(NWCreature npc, NWCreature target)
        {
            if (!npc.IsNPC) throw new Exception("Only NPCs have enmity tables.");

            EnmityTable table = GetEnmityTable(npc);

            return table.ContainsKey(target.GlobalID);
        }

        public bool IsEnmityTableEmpty(NWCreature npc)
        {
            if (!npc.IsNPC) throw new Exception("Only NPCs have enmity tables.");

            EnmityTable table = GetEnmityTable(npc);
            return table.Count <= 0;
        }
    }
}
