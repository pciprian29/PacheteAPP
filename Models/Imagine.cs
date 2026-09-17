using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PacheteAPP.Models
{
    [Table("Imagini")]
    public class Imagine
    {
        [Key]
        public int id_imagine { get; set; }
        [Required]
        [MaxLength(255)]
        public string nume_fisier { get; set; }
        public int id_tip_imagine { get; set; }
        public int id_entitate { get; set; }
        public int id_user { get; set;}
        public DateTime data_creare { get; set; }
    }
}
