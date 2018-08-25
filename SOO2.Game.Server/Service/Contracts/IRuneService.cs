using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.ValueObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IRuneService
    {
        RuneSlots GetRuneSlots(NWItem item);
        CustomItemPropertyType GetRuneType(NWItem item);
        bool IsRune(NWItem item);
        string PrismaticString();
        string OnModuleExamine(string existingDescription, NWPlayer examiner, NWObject examinedObject);
    }
}