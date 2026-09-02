using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PacheteAPP.Models.Types
{
    [Table("TipLipsa")]
    public class TipLipsa
    {
        [Key]
        public int id_tip_lipsa { get; set; }
        [Required]
        public string denumire { get; set; }
    }
}
