using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.NWNX.Contracts;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using static NWN.NWScript;
using Object = NWN.Object;

namespace Freescape.Game.Server.Event.Module
{
    public class OnModuleExamine: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IFarmingService _farming;
        private readonly IDurabilityService _durability;
        private readonly IPerkService _perk;
        private readonly IItemService _item;
        private readonly INWNXEvents _nwnxEvents;
        private readonly IExaminationService _examination;

        public OnModuleExamine(
            INWScript script,
            IFarmingService farming,
            IDurabilityService durability,
            IPerkService perk,
            IItemService item,
            INWNXEvents nwnxEvents,
            IExaminationService examination)
        {
            _ = script;
            _farming = farming;
            _durability = durability;
            _perk = perk;
            _item = item;
            _nwnxEvents = nwnxEvents;
            _examination = examination;
        }

        public bool Run(params object[] args)
        {
            NWPlayer examiner = NWPlayer.Wrap(Object.OBJECT_SELF);
            NWObject examinedObject = _nwnxEvents.OnExamineObject_GetTarget();
            if (_examination.OnModuleExamine(examiner, examinedObject)) return true;

            string description = _.GetDescription(examinedObject.Object, TRUE) + "\n\n";
            description = _item.OnModuleExamine(description, examiner, examinedObject);
            description = _perk.OnModuleExamine(description, examiner, examinedObject);
            description = _durability.OnModuleExamine(description, examinedObject);
            description = _farming.OnModuleExamine(description, examinedObject);

            if (string.IsNullOrWhiteSpace(description)) return false;
            _.SetDescription(examinedObject.Object, description, FALSE);
            _.SetDescription(examinedObject.Object, description);
            

            return true;
        }
    }
}
