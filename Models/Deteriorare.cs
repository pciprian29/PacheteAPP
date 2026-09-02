using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PacheteAPP.Models
{
    [Table("Deteriorari")]
    public class Deteriorare
    {
        [Key]
        public int id_deteriorare { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string locatie_deteriorare { get; set; }

        [Required]
        [MaxLength(255)]
        public string descriere_deteriorare { get; set; }

        [Required]
        public int id_inregistrare { get; set; }
    }
}
