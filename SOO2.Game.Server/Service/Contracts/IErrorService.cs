using System;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IErrorService
    {
        void LogError(Exception ex, string @event = "");
    }
}
