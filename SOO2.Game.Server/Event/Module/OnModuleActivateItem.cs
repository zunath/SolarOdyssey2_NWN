using Freescape.Game.Server.Service.Contracts;
using NWN;
using Object = NWN.Object;

namespace Freescape.Game.Server.Event.Module
{
    internal class OnModuleActivateItem : IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IItemService _item;
        private readonly IAbilityService _ability;

        public OnModuleActivateItem(
            INWScript script,
            IItemService item,
            IAbilityService ability)
        {
            _ = script;
            _item = item;
            _ability = ability;
        }

        public bool Run(params object[] args)
        {
            _.ExecuteScript("x2_mod_def_act", Object.OBJECT_SELF);
            _item.OnModuleActivatedItem();
            _ability.OnModuleItemActivated();

            return true;
        }


    }
}
