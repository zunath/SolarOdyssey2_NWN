﻿using System;

namespace Freescape.Game.Server.Service.Contracts
{
    public interface ITimeService
    {
        string GetTimeToWaitLongIntervals(DateTime firstDate, DateTime secondDate, bool showIfZero);
        string GetTimeToWaitShortIntervals(DateTime firstDate, DateTime secondDate, bool showIfZero);
    }
}