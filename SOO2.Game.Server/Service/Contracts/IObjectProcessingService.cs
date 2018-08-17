using System;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface IObjectProcessingService
    {
        void OnModuleLoad();

        float ProcessingTickInterval { get; }
        string RegisterProcessingEvent(Action action);
        void UnregisterProcessingEvent(string globalID);
    }
}