using System.ComponentModel.DataAnnotations;

namespace SOO2.Game.Server.Data.Entities
{
    public partial class Item
    {
        [Key]
        [StringLength(16)]
        public string Resref { get; set; }

        public int AC { get; set; }

        public int ItemTypeID { get; set; }

        public int RecommendedLevel { get; set; }

        public int LoggingBonus { get; set; }

        public int MiningBonus { get; set; }

        public int CastingSpeed { get; set; }

        public int CraftBonusMetalworking { get; set; }

        public int CraftBonusWeaponsmith { get; set; }

        public int CraftBonusArmorsmith { get; set; }

        public int CraftBonusCooking { get; set; }

        public int DurabilityPoints { get; set; }

        public int AssociatedSkillID { get; set; }

        public int CraftTierLevel { get; set; }

        public int CraftBonusEngineering { get; set; }

        public int Weight { get; set; }

        public int HPBonus { get; set; }

        public int ManaBonus { get; set; }

        public int EnmityRate { get; set; }

        public int EvocationBonus { get; set; }

        public int AlterationBonus { get; set; }

        public int SummoningBonus { get; set; }

        public int LuckBonus { get; set; }

        public int MeditateBonus { get; set; }

        public int FirstAidBonus { get; set; }

        public int HPRegenBonus { get; set; }

        public int ManaRegenBonus { get; set; }

        public int BaseAttackBonus { get; set; }

        public virtual ItemType ItemType { get; set; }

        public virtual Skill Skill { get; set; }
    }
}
