﻿using Freescape.Game.Server;
using Freescape.Game.Server.Event.Dialog;

// ReSharper disable once CheckNamespace
namespace NWN.Scripts
{
#pragma warning disable IDE1006 // Naming Styles
    internal class dialog_appears_3
#pragma warning restore IDE1006 // Naming Styles
    {
        // ReSharper disable once UnusedMember.Local
        private static int Main()
        {
            return App.RunEvent<AppearsWhen>(2, 3) ? 1 : 0;
        }
    }
}