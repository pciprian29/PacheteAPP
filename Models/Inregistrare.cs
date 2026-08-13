using System.ComponentModel.DataAnnotations;

namespace PacheteAPP.Models
{
    public class Inregistrare
    {
        [Key]
        public int id_inregistrare { get; set; }
        [Required]
        public TipInregistrare tip_inregistrare { get; set; }

        [Required]
        public int id_user { get; set; }

        [Required]
        public int id_pachet { get; set; }
    }
}