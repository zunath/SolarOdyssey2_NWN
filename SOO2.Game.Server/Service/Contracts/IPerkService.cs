using System.Collections.Generic;
using SOO2.Game.Server.Data.Entities;
using SOO2.Game.Server.Enumeration;
using SOO2.Game.Server.GameObject;
using SOO2.Game.Server.ValueObject;

namespace SOO2.Game.Server.Service.Contracts
{
    public interface IPerkService
    {
        void OnModuleItemEquipped();
        void OnModuleItemUnequipped();
        int GetPCPerkLevel(NWPlayer player, PerkType perkType);
        int GetPCPerkLevel(NWPlayer player, int perkTypeID);
        int GetPCTotalPerkCount(string playerID);
        List<PCPerkHeader> GetPCPerksForMenuHeader(string playerID);
        List<PerkCategory> GetPerkCategoriesForPC(string playerID);
        List<Data.Entities.Perk> GetPerksForPC(string playerID, int categoryID);
        Data.Entities.Perk GetPerkByID(int perkID);
        PCPerk GetPCPerkByID(string playerID, int perkID);
        PerkLevel FindPerkLevel(IEnumerable<PerkLevel> levels, int findLevel);
        bool CanPerkBeUpgraded(Data.Entities.Perk perk, PCPerk pcPerk, PlayerCharacter player);
        void DoPerkUpgrade(NWPlayer player, int perkID);
        void DoPerkUpgrade(NWPlayer player, PerkType perkType);
        string OnModuleExamine(string existingDescription, NWPlayer examiner, NWObject examinedObject);
        void OnHitCastSpell(NWPlayer oPC);
    }
}
