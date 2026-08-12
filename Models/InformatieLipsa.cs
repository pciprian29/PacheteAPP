using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PacheteAPP.Models
{
    [Table("InformatiiLipsa")]
    public class InformatieLipsa
    {
        [Key]
        public int id_inregistrare { get; set; }

        [Required]
        [MaxLength(255)]
        public string camp_afectat { get; set; }

        [Required]
        public TipLipsa tip_lipsa { get; set; }

        [Required]
        [MaxLength(255)]
        public string descriere { get; set; }

        [Required]
        public int pachet_afectat { get; set; }
    }
}
