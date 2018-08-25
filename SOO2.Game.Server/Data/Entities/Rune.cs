using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOO2.Game.Server.Data.Entities
{
    public partial class Rune
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Rune()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RuneID { get; set; }
        
        public string Name { get; set; }

        public string Script { get; set; }

        public bool IsActive { get; set; }

    }
}
