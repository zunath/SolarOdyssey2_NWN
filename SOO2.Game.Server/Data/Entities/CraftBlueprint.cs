using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Freescape.Game.Server.Data.Entities
{
    public partial class CraftBlueprint
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CraftBlueprint()
        {
            CraftBlueprintComponents = new HashSet<CraftBlueprintComponent>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long CraftBlueprintID { get; set; }

        public long CraftCategoryID { get; set; }

        public int Level { get; set; }

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

        public int CraftTierLevel { get; set; }

        public virtual CraftBlueprintCategory CraftBlueprintCategory { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CraftBlueprintComponent> CraftBlueprintComponents { get; set; }

        public virtual CraftDevice CraftDevice { get; set; }

        public virtual Perk Perk { get; set; }

        public virtual Skill Skill { get; set; }
    }
}
