using NWN;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject;

namespace SOO2.Game.Server.Bioware.Contracts
{
    public interface IBiowareXP2
    {
        void IPSafeAddItemProperty(NWItem oItem, ItemProperty ip, float fDuration, AddItemPropertyPolicy nAddItemPropertyPolicy, bool bIgnoreDurationType, bool bIgnoreSubType);
        void IPRemoveMatchingItemProperties(NWItem oItem, int nItemPropertyType, int nItemPropertyDuration, int nItemPropertySubType);
        bool IPGetItemHasProperty(NWItem oItem, ItemProperty ipCompareTo, int nDurationCompare, bool bIgnoreSubType);
        void IPRemoveAllItemProperties(NWItem oItem, int nItemPropertyDuration);
    }
}