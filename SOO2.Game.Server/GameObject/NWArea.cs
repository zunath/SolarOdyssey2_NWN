using Freescape.Game.Server.GameObject.Contracts;
using NWN;
using Object = NWN.Object;

namespace Freescape.Game.Server.GameObject
{
    public class NWArea: NWObject, INWArea
    {
        public NWArea(INWScript script) 
            : base(script)
        {
        }

        public new static NWArea Wrap(Object @object)
        {
            NWArea obj = (NWArea)App.Resolve<INWArea>();
            obj.Object = @object;

            return obj;
        }
    }
}
