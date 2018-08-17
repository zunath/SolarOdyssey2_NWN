using System;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Service
{
    public class ObjectProcessingService : IObjectProcessingService
    {
        private readonly INWScript _;
        private readonly AppState _state;

        public ObjectProcessingService(INWScript script,
            AppState state)
        {
            _ = script;
            _state = state;
        }

        public void OnModuleLoad()
        {
            NWArea area = NWArea.Wrap(_.GetFirstArea());
            while (area.IsValid)
            {
                NWObject @object = NWObject.Wrap(_.GetFirstObjectInArea(area.Object));
                while (@object.IsValid)
                {
                    HandleSpawnWaypointRename(@object);

                    @object = NWObject.Wrap(_.GetNextObjectInArea(area.Object));
                }

                area = NWArea.Wrap(_.GetNextArea());
            }

            RunProcessor();
        }

        public float ProcessingTickInterval => 1f;

        private void RunProcessor()
        {
            foreach (var @event in _state.ProcessingEvents)
            {
                @event.Value.Invoke();
            }

            _.DelayCommand(ProcessingTickInterval, RunProcessor);
        }

        public string RegisterProcessingEvent(Action action)
        {
            string globalID = Guid.NewGuid().ToString();
            _state.ProcessingEvents.Add(globalID, action);
            return globalID;
        }

        public void UnregisterProcessingEvent(string globalID)
        {
            if (_state.ProcessingEvents.ContainsKey(globalID))
            {
                _state.ProcessingEvents.Remove(globalID);
            }
        }

        // It's difficult to see what waypoint represents what in the toolset.
        // To fix this, we rename the waypoints on module load so that they function in-game.
        private void HandleSpawnWaypointRename(NWObject obj)
        {
            if (obj.ObjectType != OBJECT_TYPE_WAYPOINT) return;

            string name = obj.Name;
            
            
            string[] split = name.Split(new[] {"SP_"}, StringSplitOptions.None);
            
            if (split.Length <= 1) return;

            name = "SP_" + split[split.Length - 1];
            name = name.Trim();

            obj.Name = name;
        }
    }
}
