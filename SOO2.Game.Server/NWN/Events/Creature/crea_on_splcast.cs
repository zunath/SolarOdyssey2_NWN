using Freescape.Game.Server;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.Event.Creature;

// ReSharper disable once CheckNamespace
namespace NWN.Scripts
{
#pragma warning disable IDE1006 // Naming Styles
    internal class crea_on_splcast
#pragma warning restore IDE1006 // Naming Styles
    {
        public static void Main()
        {
            App.RunEvent<OnCreatureEvent>(CreatureEventType.OnSpellCastAt);
        }
    }
}
