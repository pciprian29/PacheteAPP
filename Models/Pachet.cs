using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PacheteAPP.Models
{
    public enum StatusPachet
    {
        Inregistrat,
        InTranzit,
        Livrat,
        Probleme,
        Retur
    }
    public enum TipPachet
    {
        Plic,
        Cutie,
        Palet
    }

    public enum TipLipsa
    {
        Lipsa,
        Partial
    }
    [Table("Pachete")]
    public class Pachet 
    {
        [Key]
        public int id_pachet { get; set; }

        [Required]
        [MaxLength(50)]
        public string awb  { get; set; }

        [Required]
        [MaxLength(255)]
        public string descriere { get; set; }


        [Required]
        public TipPachet tip_pachet { get; set; }

        [Required]
        [MaxLength(50)]
        public string nume_expeditor { get; set; }

        [Required]
        [MaxLength(255)]
        public string adresa_expeditor { get; set; }

        [Required]
        [MaxLength(50)]
        public string nume_destinatar { get; set; }

        [Required]
        [MaxLength(255)]
        public string adresa_destinatar { get; set; }

        [Required]
        public DateTime data_inregistrare { get; set; } = DateTime.Now;

        [Required]
        public decimal greutate_teoretica { get; set; }

        [Required]
        public decimal greutate_efectiva { get; set; }
        
        [Required]
        public StatusPachet status_pachet { get; set; }

        [Required]
        [MaxLength(450)]
        public string id_user { get; set; }
    }
}
