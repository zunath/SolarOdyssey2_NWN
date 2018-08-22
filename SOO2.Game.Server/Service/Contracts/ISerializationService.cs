using NWN;
using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface ISerializationService
    {
        NWCreature DeserializeCreature(string base64String, Location location);
        NWItem DeserializeItem(string base64String, Location targetLocation);
        NWItem DeserializeItem(string base64String, NWPlaceable target);
        string Serialize(NWObject @object);
    }
}
