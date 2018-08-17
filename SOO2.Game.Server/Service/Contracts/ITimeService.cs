using System;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface ITimeService
    {
        string GetTimeToWaitLongIntervals(DateTime firstDate, DateTime secondDate, bool showIfZero);
        string GetTimeToWaitShortIntervals(DateTime firstDate, DateTime secondDate, bool showIfZero);
    }
}