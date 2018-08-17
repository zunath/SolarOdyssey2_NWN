﻿using System;
using Freescape.Game.Server.Extension;
using Freescape.Game.Server.Service.Contracts;

namespace Freescape.Game.Server.Service
{
    public class ErrorService: IErrorService
    {
        public void LogError(Exception ex, string @event = "")
        {
            string message = ex.ToMessageAndCompleteStacktrace();

            message = "*****************" + Environment.NewLine +
                      "EVENT ERROR (C#)" + Environment.NewLine +
                      (string.IsNullOrWhiteSpace(@event) ? string.Empty : @event + Environment.NewLine) +
                      "*****************" + Environment.NewLine +
                      " EXCEPTION:" + Environment.NewLine + Environment.NewLine + message;
            Console.WriteLine(message); // todo: log in database
        }
    }
}
