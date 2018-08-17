using System;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface IErrorService
    {
        void LogError(Exception ex, string @event = "");
    }
}
