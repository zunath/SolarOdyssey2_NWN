using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.Event;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Service.Contracts;
using NWN;
using Object = NWN.Object;

namespace Freescape.Game.Server.Placeable.StructureSystem.PersistentStorage
{
    public class OnUsed: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IStructureService _structure;
        private readonly IDialogService _dialog;

        public OnUsed(INWScript script,
            IStructureService structure,
            IDialogService dialog)
        {
            _ = script;
            _structure = structure;
            _dialog = dialog;
        }

        public bool Run(params object[] args)
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetLastUsedBy());
            NWPlaceable objSelf = NWPlaceable.Wrap(Object.OBJECT_SELF);
            int structureID = _structure.GetPlaceableStructureID(objSelf);
            PCTerritoryFlagsStructure structure = _structure.GetPCStructureByID(structureID);

            if (_structure.PlayerHasPermission(oPC, StructurePermission.CanAccessPersistentStorage, structure.PCTerritoryFlagID) ||
                _structure.PlayerHasPermission(oPC, StructurePermission.CanRenameStructures, structure.PCTerritoryFlagID))
            {
                _dialog.StartConversation(oPC, objSelf, "StructureStorage");
            }
            else
            {
                oPC.FloatingText("You do not have permission to access this structure.");
            }

            return true;
        }
    }
}
