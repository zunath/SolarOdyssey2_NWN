using SOO2.Game.Server.Event;

namespace SOO2.Game.Server.Placeable.Resource
{
    public class OnDeath: IRegisteredEvent
    {
        public bool Run(params object[] args)
        {
            return true;
        }
    }
}
