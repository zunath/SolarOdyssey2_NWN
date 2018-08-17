using System.ComponentModel.DataAnnotations;

namespace Freescape.Game.Server.Data.Entities
{
    public partial class CraftBlueprintComponent
    {
        [Key]
        public long CraftComponentID { get; set; }

        public long CraftBlueprintID { get; set; }

        [Required]
        [StringLength(16)]
        public string ItemResref { get; set; }

        public int Quantity { get; set; }

        public virtual CraftBlueprint CraftBlueprint { get; set; }
    }
}
