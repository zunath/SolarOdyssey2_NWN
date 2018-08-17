using Freescape.Game.Server.Event;

namespace Freescape.Game.Server.Placeable.Resource
{
    public class OnDeath: IRegisteredEvent
    {
        public bool Run(params object[] args)
        {
            return true;
        }
    }
}
