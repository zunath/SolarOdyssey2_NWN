using System;
using System.Linq;
using FluentBehaviourTree;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Service
{
    public class BehaviourService : IBehaviourService
    {
        private readonly IObjectProcessingService _objProc;
        private readonly AppState _state;

        public BehaviourService(IObjectProcessingService objProc,
            AppState state)
        {
            _objProc = objProc;
            _state = state;
        }
        
        public void RegisterBehaviour(IBehaviourTreeNode node, NWCreature creature)
        {
            TimeData time = new TimeData(_objProc.ProcessingTickInterval);
            string behaviourID = _objProc.RegisterProcessingEvent(() =>
            {
                if (creature.IsValid)
                {
                    node.Tick(time);
                }
            });
            _state.NPCBehaviours.Add(behaviourID, creature);
        }
        
    }
}
