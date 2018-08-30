using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOO2.Game.Server.Data.Entities
{
    public partial class CraftBlueprint
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CraftBlueprint()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long CraftBlueprintID { get; set; }

        public long CraftCategoryID { get; set; }

        public int BaseLevel { get; set; }

        [Required]
        [StringLength(64)]
        public string ItemName { get; set; }

        [Required]
        [StringLength(16)]
        public string ItemResref { get; set; }

        public int Quantity { get; set; }

        public int SkillID { get; set; }

        public int CraftDeviceID { get; set; }

        public int? PerkID { get; set; }

        public int RequiredPerkLevel { get; set; }

        public bool IsActive { get; set; }
        
        public virtual CraftBlueprintCategory CraftBlueprintCategory { get; set; }
        
        public virtual CraftDevice CraftDevice { get; set; }

        public virtual Perk Perk { get; set; }

        public virtual Skill Skill { get; set; }

        public int MainComponentTypeID { get; set; }
        
        public virtual ComponentType MainComponentType { get; set; }

        public int MainMinimum { get; set; }

        public int SecondaryComponentTypeID { get; set; }

        public virtual ComponentType SecondaryComponentType { get; set; }

        public int SecondaryMinimum { get; set; }
        
        public int TertiaryComponentTypeID { get; set; }

        public virtual ComponentType TertiaryComponentType { get; set; }

        public int TertiaryMinimum { get; set; }

        public int EnhancementSlots { get; set; }
    }
}
