using NWN;
using SOO2.Game.Server.Data.Entities;
using SOO2.Game.Server.Event;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.Service.Contracts;

namespace SOO2.Game.Server.Placeable.StructureSystem.PersistentStorage
{
    public class OnOpened: IRegisteredEvent
    {
        private readonly INWScript _;
        private readonly IStructureService _structure;
        private readonly ISerializationService _serialization;

        public OnOpened(INWScript script,
            IStructureService structure,
            ISerializationService serialization)
        {
            _ = script;
            _structure = structure;
            _serialization = serialization;
        }

        public bool Run(params object[] args)
        {
            NWPlaceable chest = NWPlaceable.Wrap(Object.OBJECT_SELF);
            int structureID = chest.GetLocalInt("STRUCTURE_TEMP_STRUCTURE_ID");
            PCTerritoryFlagsStructure entity = _structure.GetPCStructureByID(structureID);
            Location location = chest.Location;

            foreach (PCTerritoryFlagsStructuresItem item in entity.PCTerritoryFlagsStructuresItems)
            {
                _serialization.DeserializeItem(item.ItemObject, chest);
            }

            chest.IsUseable = false;
            return true;
        }
    }
}
