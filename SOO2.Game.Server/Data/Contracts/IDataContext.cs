﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Freescape.Game.Server.Data.Entities;
using Attribute = Freescape.Game.Server.Data.Entities.Attribute;

namespace Freescape.Game.Server.Data.Contracts
{
    public interface IDataContext
    {
        IDbSet<Attribute> Attributes { get; set; }
        IDbSet<AuthorizedDM> AuthorizedDMs { get; set; }
        IDbSet<Background> Backgrounds { get; set; }
        IDbSet<BaseItemType> BaseItemTypes { get; set; }
        IDbSet<BuildingCategory> BuildingCategories { get; set; }
        IDbSet<BuildingInterior> BuildingInteriors { get; set; }
        IDbSet<BuildPrivacyDomain> BuildPrivacyDomains { get; set; }
        IDbSet<ChatChannelsDomain> ChatChannelsDomains { get; set; }
        IDbSet<ChatLog> ChatLogs { get; set; }
        IDbSet<ClientLogEvent> ClientLogEvents { get; set; }
        IDbSet<ClientLogEventTypesDomain> ClientLogEventTypesDomains { get; set; }
        IDbSet<ConstructionSiteComponent> ConstructionSiteComponents { get; set; }
        IDbSet<ConstructionSite> ConstructionSites { get; set; }
        IDbSet<CooldownCategory> CooldownCategories { get; set; }
        IDbSet<CraftBlueprintCategory> CraftBlueprintCategories { get; set; }
        IDbSet<CraftBlueprintComponent> CraftBlueprintComponents { get; set; }
        IDbSet<CraftBlueprint> CraftBlueprints { get; set; }
        IDbSet<CraftDevice> CraftDevices { get; set; }
        IDbSet<Entities.CustomEffect> CustomEffects { get; set; }
        IDbSet<DMRoleDomain> DMRoleDomains { get; set; }
        IDbSet<Download> Downloads { get; set; }
        IDbSet<FameRegion> FameRegions { get; set; }
        IDbSet<GameTopicCategory> GameTopicCategories { get; set; }
        IDbSet<GameTopic> GameTopics { get; set; }
        IDbSet<GrowingPlant> GrowingPlants { get; set; }
        IDbSet<Entities.Item> Items { get; set; }
        IDbSet<ItemType> ItemTypes { get; set; }
        IDbSet<KeyItemCategory> KeyItemCategories { get; set; }
        IDbSet<KeyItem> KeyItems { get; set; }
        IDbSet<LootTableItem> LootTableItems { get; set; }
        IDbSet<LootTable> LootTables { get; set; }
        IDbSet<NPCGroup> NPCGroups { get; set; }
        IDbSet<PCCooldown> PCCooldowns { get; set; }
        IDbSet<PCCorpseItem> PCCorpseItems { get; set; }
        IDbSet<PCCorpse> PCCorpses { get; set; }
        IDbSet<PCCustomEffect> PCCustomEffects { get; set; }
        IDbSet<PCKeyItem> PCKeyItems { get; set; }
        IDbSet<PCMapPin> PCMapPins { get; set; }
        IDbSet<PCMigrationItem> PCMigrationItems { get; set; }
        IDbSet<PCMigration> PCMigrations { get; set; }
        IDbSet<PCOutfit> PCOutfits { get; set; }
        IDbSet<PCOverflowItem> PCOverflowItems { get; set; }
        IDbSet<PCPerk> PCPerks { get; set; }
        IDbSet<PCQuestKillTargetProgress> PCQuestKillTargetProgresses { get; set; }
        IDbSet<PCQuestStatus> PCQuestStatus { get; set; }
        IDbSet<PCRegionalFame> PCRegionalFames { get; set; }
        IDbSet<PCSearchSiteItem> PCSearchSiteItems { get; set; }
        IDbSet<PCSearchSite> PCSearchSites { get; set; }
        IDbSet<PCSkill> PCSkills { get; set; }
        IDbSet<PCTerritoryFlag> PCTerritoryFlags { get; set; }
        IDbSet<PCTerritoryFlagsPermission> PCTerritoryFlagsPermissions { get; set; }
        IDbSet<PCTerritoryFlagsStructure> PCTerritoryFlagsStructures { get; set; }
        IDbSet<PCTerritoryFlagsStructuresItem> PCTerritoryFlagsStructuresItems { get; set; }
        IDbSet<PerkCategory> PerkCategories { get; set; }
        IDbSet<PerkExecutionType> PerkExecutionTypes { get; set; }
        IDbSet<PerkLevel> PerkLevels { get; set; }
        IDbSet<PerkLevelSkillRequirement> PerkLevelSkillRequirements { get; set; }
        IDbSet<Entities.Perk> Perks { get; set; }
        IDbSet<Plant> Plants { get; set; }
        IDbSet<PlayerCharacter> PlayerCharacters { get; set; }
        IDbSet<QuestKillTargetList> QuestKillTargetLists { get; set; }
        IDbSet<QuestPrerequisite> QuestPrerequisites { get; set; }
        IDbSet<QuestRequiredItemList> QuestRequiredItemLists { get; set; }
        IDbSet<QuestRequiredKeyItemList> QuestRequiredKeyItemLists { get; set; }
        IDbSet<QuestRewardItem> QuestRewardItems { get; set; }
        IDbSet<Quest> Quests { get; set; }
        IDbSet<QuestState> QuestStates { get; set; }
        IDbSet<QuestTypeDomain> QuestTypeDomains { get; set; }
        IDbSet<ServerConfiguration> ServerConfigurations { get; set; }
        IDbSet<SkillCategory> SkillCategories { get; set; }
        IDbSet<Skill> Skills { get; set; }
        IDbSet<SkillXPRequirement> SkillXPRequirements { get; set; }
        IDbSet<StorageContainer> StorageContainers { get; set; }
        IDbSet<StorageItem> StorageItems { get; set; }
        IDbSet<StructureBlueprint> StructureBlueprints { get; set; }
        IDbSet<StructureCategory> StructureCategories { get; set; }
        IDbSet<StructureComponent> StructureComponents { get; set; }
        IDbSet<StructureQuickBuildAudit> StructureQuickBuildAudits { get; set; }
        IDbSet<sysdiagram> sysdiagrams { get; set; }
        IDbSet<TerritoryFlagPermission> TerritoryFlagPermissions { get; set; }
        IDbSet<User> Users { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        void StoredProcedure(string procedureName, params SqlParameter[] args);
        List<T> StoredProcedure<T>(string procedureName, params SqlParameter[] args);
        T StoredProcedureSingle<T>(string procedureName, params SqlParameter[] args);
    }
}