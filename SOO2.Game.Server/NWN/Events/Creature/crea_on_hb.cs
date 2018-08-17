using SOO2.Game.Server;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.Event.Creature;

// ReSharper disable once CheckNamespace
namespace NWN.Scripts
{
#pragma warning disable IDE1006 // Naming Styles
    internal class crea_on_hb
#pragma warning restore IDE1006 // Naming Styles
    {
        public static void Main()
        {
            App.RunEvent<OnCreatureEvent>(CreatureEventType.OnHeartbeat);
        }
    }
}
