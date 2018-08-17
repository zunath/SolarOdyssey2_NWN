using System;
using FluentBehaviourTree;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Service
{
    public class BehaviourService : IBehaviourService
    {
        private readonly IObjectProcessingService _objProc;

        public BehaviourService(IObjectProcessingService objProc)
        {
            _objProc = objProc;
        }
        
        public string RegisterBehaviour(IBehaviourTreeNode node)
        {
            TimeData time = new TimeData(_objProc.ProcessingTickInterval);
            string behaviourID = _objProc.RegisterProcessingEvent(() => node.Tick(time));
            return behaviourID;
        }

        public void UnregisterBehaviour(string behaviourID)
        {
            _objProc.UnregisterProcessingEvent(behaviourID);
        }
    }
}
