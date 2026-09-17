using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PacheteAPP.Models.Types
{
    [Table("TipImagine")]
    public class TipImagine
    {
        [Key]
        public int id_tip_imagine { get; set; }
        [Required]
        [MaxLength(50)]
        public string denumire { get; set; }
    }
}
