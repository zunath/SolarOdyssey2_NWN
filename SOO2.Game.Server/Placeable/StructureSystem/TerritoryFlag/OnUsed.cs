using NWN;
using SOO2.Game.Server.Data.Entities;
using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Placeable.StructureSystem.TerritoryFlag
{
    public class OnUsed: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IDialogService _dialog;
        private readonly IStructureService _structure;

        public OnUsed(INWScript script,
            IDialogService dialog,
            IStructureService structure)
        {
            _ = script;
            _dialog = dialog;
            _structure = structure;
        }

        public bool Run(params object[] args)
        {
            NWPlaceable site = NWPlaceable.Wrap(Object.OBJECT_SELF);
            NWPlayer oPC = NWPlayer.Wrap(_.GetLastUsedBy());
            int flagID = _structure.GetTerritoryFlagID(site);
            PCTerritoryFlag entity = _structure.GetPCTerritoryFlagByID(flagID);

            if (oPC.GlobalID != entity.PlayerID && !oPC.IsDM)
            {
                oPC.SendMessage("Only the owner of the territory may use this.");
            }
            else
            {
                _dialog.StartConversation(oPC, site, "TerritoryFlag");
            }
            return true;
        }
    }
}
