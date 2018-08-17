﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Freescape.Game.Server.Bioware.Contracts;
using Freescape.Game.Server.Data.Contracts;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.Perk;
using Freescape.Game.Server.Service.Contracts;
using Freescape.Game.Server.ValueObject;
using NWN;
using static NWN.NWScript;
using PerkExecutionType = Freescape.Game.Server.Enumeration.PerkExecutionType;

namespace Freescape.Game.Server.Service
{
    public class PerkService: IPerkService
    {
        private readonly INWScript _;
        private readonly IColorTokenService _color;
        private readonly IDataContext _db;
        private readonly IBiowareXP2 _biowareXP2;

        public PerkService(INWScript script,
            IColorTokenService color,
            IDataContext db,
            IBiowareXP2 biowareXP2)
        {
            _ = script;
            _color = color;
            _db = db;
            _biowareXP2 = biowareXP2;
        }

        public void OnModuleItemEquipped()
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetPCItemLastEquippedBy());
            NWItem oItem = NWItem.Wrap(_.GetPCItemLastEquipped());
            if (!oPC.IsPlayer || !oPC.IsInitializedAsPlayer) return;
            List<PCPerk> perks = _db.StoredProcedure<PCPerk>("GetPCPerksByExecutionType",
                new SqlParameter("PlayerID", oPC.GlobalID),
                new SqlParameter("ExecutionTypeID", (int) PerkExecutionType.EquipmentBased));

            foreach (PCPerk pcPerk in perks)
            {
                pcPerk.Perk = _db.Perks.Single(x => x.PerkID == pcPerk.PerkID);
                string jsName = pcPerk.Perk.JavaScriptName;
                if (string.IsNullOrWhiteSpace(jsName)) continue;

                IPerk perkAction = App.ResolveByInterface<IPerk>("Perk." + jsName);
                perkAction?.OnItemEquipped(oPC, oItem);
            }
        }

        public void OnModuleItemUnequipped()
        {
            NWPlayer oPC = NWPlayer.Wrap(_.GetPCItemLastUnequippedBy());
            NWItem oItem = NWItem.Wrap(_.GetPCItemLastUnequipped());
            if (!oPC.IsPlayer) return;

            List<PCPerk> perks = _db.StoredProcedure<PCPerk>("GetPCPerksByExecutionType",
                new SqlParameter("PlayerID", oPC.GlobalID),
                new SqlParameter("ExecutionTypeID", (int)PerkExecutionType.EquipmentBased));

            foreach (PCPerk pcPerk in perks)
            {
                pcPerk.Perk = _db.Perks.Single(x => x.PerkID == pcPerk.PerkID);
                string jsName = pcPerk.Perk.JavaScriptName;
                if (string.IsNullOrWhiteSpace(jsName)) continue;
                
                IPerk perkAction = App.ResolveByInterface<IPerk>("Perk." + jsName);
                perkAction?.OnItemUnequipped(oPC, oItem);
            }
        }

        public int GetPCPerkLevel(NWPlayer player, PerkType perkType)
        {
            return GetPCPerkLevel(player, (int)perkType);
        }

        public int GetPCPerkLevel(NWPlayer player, int perkID)
        {
            if (!player.IsPlayer) return -1;
            
            PerkLevel perkLevel = _db.StoredProcedureSingle<PerkLevel>("GetPCSkillAdjustedPerkLevel",
                new SqlParameter("PlayerID", player.GlobalID),
                new SqlParameter("PerkID", perkID));

            return perkLevel?.Level ?? 0;
        }

        public void OnHitCastSpell(NWPlayer oPC)
        {
            if (!oPC.IsPlayer) return;
            NWItem oItem = NWItem.Wrap(_.GetSpellCastItem());
            int type = oItem.BaseItemType;
            List<PCPerk> pcPerks = _db.StoredProcedure<PCPerk>("GetPCPerksWithExecutionType",
                new SqlParameter("PlayerID", oPC.GlobalID));
            
            foreach (PCPerk pcPerk in pcPerks)
            {
                pcPerk.Perk = GetPerkByID(pcPerk.PerkID);
                if (string.IsNullOrWhiteSpace(pcPerk.Perk.JavaScriptName) || pcPerk.Perk.ExecutionTypeID == (int)PerkExecutionType.None) continue;

                IPerk perkAction = App.ResolveByInterface<IPerk>("Perk." +pcPerk.Perk.JavaScriptName);
                if (perkAction == null) continue;
                
                if (pcPerk.Perk.ExecutionTypeID == (int)PerkExecutionType.ShieldOnHit)
                {
                    if (type == BASE_ITEM_SMALLSHIELD || type == BASE_ITEM_LARGESHIELD || type == BASE_ITEM_TOWERSHIELD)
                    {
                        perkAction.OnImpact(oPC, oItem);
                    }
                }
            }
        }

        public int GetPCTotalPerkCount(string playerID)
        {
            return _db.PCPerks.Count(x => x.PlayerID == playerID);
        }

        public List<PCPerkHeader> GetPCPerksForMenuHeader(string playerID)
        {
            return _db.StoredProcedure<PCPerkHeader>("GetPCPerksForMenuHeader",
                    new SqlParameter("PlayerID", playerID)); ;
        }

        public List<PerkCategory> GetPerkCategoriesForPC(string playerID)
        {
            return _db.StoredProcedure<PerkCategory>("GetPerkCategoriesForPC",
                new SqlParameter("PlayerID", playerID));
        }

        public List<Data.Entities.Perk> GetPerksForPC(string playerID, int categoryID)
        {
            return _db.StoredProcedure<Data.Entities.Perk>("GetPerksForPC",
                new SqlParameter("PlayerID", playerID),
                new SqlParameter("CategoryID", categoryID));
        }

        public Data.Entities.Perk GetPerkByID(int perkID)
        {
            return _db.Perks.Single(x => x.PerkID == perkID);
        }

        public PCPerk GetPCPerkByID(string playerID, int perkID)
        {
            return _db.PCPerks.SingleOrDefault(x => x.PlayerID == playerID && x.PerkID == perkID);
        }

        public PerkLevel FindPerkLevel(IEnumerable<PerkLevel> levels, int findLevel)
        {
            return levels.FirstOrDefault(lvl => lvl.Level == findLevel);
        }

        public bool CanPerkBeUpgraded(Data.Entities.Perk perk, PCPerk pcPerk, PlayerCharacter player)
        {
            int rank = pcPerk?.PerkLevel ?? 0;
            int maxRank = perk.PerkLevels.Count;
            if (rank + 1 > maxRank) return false;

            PerkLevel level = FindPerkLevel(perk.PerkLevels, rank + 1);
            if (level == null) return false;

            if (player.UnallocatedSP < level.Price) return false;

            foreach (PerkLevelSkillRequirement req in level.PerkLevelSkillRequirements)
            {
                PCSkill pcSkill = _db.PCSkills.Single(x => x.PlayerID == player.PlayerID && x.SkillID == req.SkillID);
                if (pcSkill.Rank < req.RequiredRank) return false;
            }

            return true;
        }

        public void DoPerkUpgrade(NWPlayer oPC, int perkID)
        {
            Data.Entities.Perk perk = _db.Perks.Single(x => x.PerkID == perkID);
            PCPerk pcPerk = _db.PCPerks.SingleOrDefault(x => x.PlayerID == oPC.GlobalID && x.PerkID == perkID);
            PlayerCharacter player = _db.PlayerCharacters.Single(x => x.PlayerID == oPC.GlobalID);

            if (CanPerkBeUpgraded(perk, pcPerk, player))
            {
                if (pcPerk == null)
                {
                    pcPerk = new PCPerk();
                    DateTime dt = DateTime.UtcNow;
                    pcPerk.AcquiredDate = dt;
                    pcPerk.PerkID = perk.PerkID;
                    pcPerk.PlayerID = oPC.GlobalID;
                    pcPerk.PerkLevel = 0;

                    _db.PCPerks.Add(pcPerk);
                }

                PerkLevel nextPerkLevel = FindPerkLevel(perk.PerkLevels, pcPerk.PerkLevel + 1);
                if (nextPerkLevel == null) return;

                pcPerk.PerkLevel++;
                player.UnallocatedSP -= nextPerkLevel.Price;

                _db.SaveChanges();

                // If a perk is activatable, create the item on the PC.
                // Remove any existing cast spell unique power properties and add the correct one based on the DB flag.
                if (!string.IsNullOrWhiteSpace(perk.ItemResref))
                {
                    if (_.GetIsObjectValid(_.GetItemPossessedBy(oPC.Object, perk.ItemResref)) == FALSE)
                    {
                        NWItem spellItem = NWItem.Wrap(_.CreateItemOnObject(perk.ItemResref, oPC.Object));
                        spellItem.IsCursed = true;
                        spellItem.SetLocalInt("ACTIVATION_PERK_ID", perk.PerkID);

                        foreach (ItemProperty ipCur in spellItem.ItemProperties)
                        {
                            int ipType = _.GetItemPropertyType(ipCur);
                            int ipSubType = _.GetItemPropertySubType(ipCur);
                            if (ipType == ITEM_PROPERTY_CAST_SPELL &&
                                    (ipSubType == IP_CONST_CASTSPELL_UNIQUE_POWER ||
                                            ipSubType == IP_CONST_CASTSPELL_UNIQUE_POWER_SELF_ONLY ||
                                            ipSubType == IP_CONST_CASTSPELL_ACTIVATE_ITEM))
                            {
                                _.RemoveItemProperty(spellItem.Object, ipCur);
                            }
                        }
                        
                        ItemProperty ip;
                        if (perk.IsTargetSelfOnly) ip = _.ItemPropertyCastSpell(IP_CONST_CASTSPELL_UNIQUE_POWER_SELF_ONLY, IP_CONST_CASTSPELL_NUMUSES_UNLIMITED_USE);
                        else ip = _.ItemPropertyCastSpell(IP_CONST_CASTSPELL_UNIQUE_POWER, IP_CONST_CASTSPELL_NUMUSES_UNLIMITED_USE);

                        _biowareXP2.IPSafeAddItemProperty(spellItem, ip, 0.0f, AddItemPropertyPolicy.ReplaceExisting, false, false);
                    }

                    _.SetName(_.GetItemPossessedBy(oPC.Object, perk.ItemResref), perk.Name + " (Lvl. " + pcPerk.PerkLevel + ")");
                }

                oPC.SendMessage(_color.Green("Perk Purchased: " + perk.Name + " (Lvl. " + pcPerk.PerkLevel + ")"));

                IPerk perkScript = App.ResolveByInterface<IPerk>("Perk." + perk.JavaScriptName);

                if (perkScript == null) return;
                perkScript.OnPurchased(oPC, pcPerk.PerkLevel);
            }
            else
            {
                oPC.FloatingText(_color.Red("You cannot purchase the perk at this time."));
            }
        }

        public void DoPerkUpgrade(NWPlayer player, PerkType perkType)
        {
            DoPerkUpgrade(player, (int)perkType);
        }

        public string OnModuleExamine(string existingDescription, NWPlayer examiner, NWObject examinedObject)
        {
            if (!examiner.IsPlayer && !examiner.IsDM) return existingDescription;
            if (examinedObject.ObjectType != OBJECT_TYPE_ITEM) return existingDescription;
            int perkID = examinedObject.GetLocalInt("ACTIVATION_PERK_ID");
            if (perkID <= 0) return existingDescription;

            Data.Entities.Perk perk = _db.Perks.Single(x => x.PerkID == perkID);
            string description = existingDescription;

            description += _color.Orange("Name: ") + perk.Name + "\n" +
                _color.Orange("Description: ") + perk.Description + "\n";

            switch ((PerkExecutionType)perk.PerkExecutionType.PerkExecutionTypeID)
            {
                case PerkExecutionType.CombatAbility:
                    description += _color.Orange("Type: ") + "Combat Ability\n";
                    break;
                case PerkExecutionType.Spell:
                    description += _color.Orange("Type: ") + "Spell\n";
                    break;
                case PerkExecutionType.ShieldOnHit:
                    description += _color.Orange("Type: ") + "Reactive\n";
                    break;
                case PerkExecutionType.QueuedWeaponSkill:
                    description += _color.Orange("Type: ") + "Queued Attack\n";
                    break;
            }

            if (perk.BaseManaCost > 0)
            {
                description += _color.Orange("Base Mana Cost: ") + perk.BaseManaCost + "\n";
            }
            if (perk.CooldownCategory.BaseCooldownTime > 0.0f)
            {
                description += _color.Orange("Cooldown: ") + perk.CooldownCategory.BaseCooldownTime + "s\n";
            }
            if (perk.BaseCastingTime > 0.0f)
            {
                description += _color.Orange("Base Casting Time: ") + perk.BaseCastingTime + "s\n";
            }


            return description;
        }
    }
}
