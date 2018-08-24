﻿using System;
using System.Linq;
using NWN;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Service
{
    public class AppStateService : IAppStateService
    {
        private readonly IObjectProcessingService _ops;
        private readonly AppState _state;
        
        public AppStateService(IObjectProcessingService ops,
            AppState state)
        {
            _ops = ops;
            _state = state;
        }

        public void OnModuleLoad()
        {
            _ops.RegisterProcessingEvent(() =>
            {

                foreach (var npcTable in _state.NPCEnmityTables.ToArray())
                {
                    if (!npcTable.Value.NPCObject.IsValid)
                    {
                        _state.NPCEnmityTables.Remove(npcTable.Key);
                    }
                }
                for (int x = _state.NPCBehaviours.Count - 1; x >= 0; x--)
                {
                    var npcBehaviour = _state.NPCBehaviours.ElementAt(x);
                    if (!npcBehaviour.Value.IsValid)
                    {
                        _state.NPCBehaviours.Remove(npcBehaviour.Key);
                        _ops.UnregisterProcessingEvent(npcBehaviour.Key);
                    }
                }
                foreach (var customData in _state.CustomObjectData.ToArray())
                {
                    NWObject owner = customData.Value.Owner;
                    if (!owner.IsValid)
                    {
                        _state.CustomObjectData.Remove(customData.Key);
                    }
                }

                //int count = NWModule.Get().GetLocalInt("current_count") + 1;
                //if (count >= 20)
                //{
                //    Console.WriteLine("******************");

                //    Console.WriteLine("PlayerDialogs: " + _state.PlayerDialogs.Count);
                //    Console.WriteLine("DialogFilesInUse: " + _state.DialogFilesInUse.Count);
                //    Console.WriteLine("EffectTicks: " + _state.EffectTicks.Count);
                //    Console.WriteLine("CreatureSkillRegistrations: " + _state.CreatureSkillRegistrations.Count);
                //    Console.WriteLine("NPCEffects: " + _state.NPCEffects.Count);
                //    Console.WriteLine("EffectsToRemove: " + _state.EffectsToRemove.Count);
                //    Console.WriteLine("ProcessingEvents: " + _state.ProcessingEvents.Count);
                //    Console.WriteLine("UnregisterProcessingEvents: " + _state.UnregisterProcessingEvents.Count);
                //    Console.WriteLine("NPCEnmityTables: " + _state.NPCEnmityTables.Count);
                //    Console.WriteLine("CustomObjectData: " + _state.CustomObjectData.Count);
                //    Console.WriteLine("NPCBehaviours: " + _state.NPCBehaviours.Count);

                //    Console.WriteLine("******************");
                //    count = 0;
                //}
                //NWModule.Get().SetLocalInt("current_count", count);

            });
        }
    }
}
