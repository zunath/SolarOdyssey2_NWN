﻿using Freescape.Game.Server;
using Freescape.Game.Server.Event.Legacy;

// ReSharper disable once CheckNamespace
namespace NWN.Scripts
{
#pragma warning disable IDE1006 // Naming Styles
    internal class jvm_script_2
#pragma warning restore IDE1006 // Naming Styles
    {
        // ReSharper disable once UnusedMember.Local
        private static void Main()
        {
            App.RunEvent<LegacyJVMEvent>("JAVA_SCRIPT_2");
        }
    }
}