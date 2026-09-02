using System.ComponentModel.DataAnnotations;

namespace PacheteAPP.Models
{
    public class Inregistrare
    {
        [Key]
        public int id_inregistrare { get; set; }
       
        [Required]
        public int id_user { get; set; }

        [Required]
        public int id_pachet { get; set; }

        public DateTime? data_inregistrare { get; set; } = DateTime.Now;
        [Required]
        public int id_tip_inregistrare { get; set; }
    }
}