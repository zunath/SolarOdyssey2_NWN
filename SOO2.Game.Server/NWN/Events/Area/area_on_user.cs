﻿using Freescape.Game.Server;
using Freescape.Game.Server.Event.Area;

// ReSharper disable once CheckNamespace
namespace NWN.Scripts
{
#pragma warning disable IDE1006 // Naming Styles
    internal class area_on_user
#pragma warning restore IDE1006 // Naming Styles
    {
        public static void Main()
        {
            App.RunEvent<OnAreaUserDefined>();
        }
    }
}
