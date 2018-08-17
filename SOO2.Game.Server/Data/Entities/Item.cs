using System.ComponentModel.DataAnnotations;

namespace Freescape.Game.Server.Data.Entities
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

        public int CraftBonusWoodworking { get; set; }

        public int Weight { get; set; }

        public int HPBonus { get; set; }

        public int ManaBonus { get; set; }

        public virtual ItemType ItemType { get; set; }

        public virtual Skill Skill { get; set; }
    }
}
