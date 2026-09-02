using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PacheteAPP.Models
{
    [Table("Pachete")]
    public class Pachet 
    {
        [Key]
        public int id_pachet { get; set; }

        [MaxLength(50)]
        public string? awb  { get; set; }

        [Required]
        [MaxLength(255)]
        public string descriere { get; set; }

        [Required]
        [MaxLength(255)]
        public string adresa_expeditor { get; set; }

        [Required]
        [MaxLength(255)]
        public string adresa_destinatar { get; set; }


        [Required]
        public decimal greutate_teoretica { get; set; }

        [Required]
        public decimal greutate_efectiva { get; set; }

        [Required]
        public int id_tip_pachet { get; set; }

        [Required]
        public int id_status_pachet { get; set; }

        [Required]
        public string email_expeditor { get; set; }

        [Required]
        public string email_destinatar { get; set; }

        [Required]
        public int numar_deteriorari { get; set; }

        [Required]
        public int numar_infolipsa { get; set; }
    }
}
