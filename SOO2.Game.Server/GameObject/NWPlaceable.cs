using Freescape.Game.Server.GameObject.Contracts;
using NWN;

namespace Freescape.Game.Server.GameObject
{
    public class NWPlaceable: NWObject, INWPlaceable
    {
        public NWPlaceable(INWScript script) 
            : base(script)
        {
        }
        
        public new static NWPlaceable Wrap(Object @object)
        {
            var obj = (NWPlaceable)App.Resolve<INWPlaceable>();
            obj.Object = @object;

            return obj;
        }

        public virtual bool IsUseable
        {
            get => _.GetUseableFlag(Object) == 1;
            set => _.SetUseableFlag(Object, value ? 1 : 0);
        }

    }
}
