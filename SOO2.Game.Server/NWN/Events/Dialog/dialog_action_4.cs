﻿using SOO2.Game.Server;
using SOO2.Game.Server.Event.Dialog;

// ReSharper disable once CheckNamespace
namespace NWN.Scripts
{
#pragma warning disable IDE1006 // Naming Styles
    internal class dialog_action_4
#pragma warning restore IDE1006 // Naming Styles
    {
        // ReSharper disable once UnusedMember.Local
        private static void Main()
        {
            App.RunEvent<ActionTaken>(4);
        }
    }
}