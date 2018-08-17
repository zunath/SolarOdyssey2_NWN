using System;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IObjectProcessingService
    {
        void OnModuleLoad();

        float ProcessingTickInterval { get; }
        string RegisterProcessingEvent(Action action);
        void UnregisterProcessingEvent(string globalID);
    }
}