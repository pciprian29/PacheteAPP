using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PacheteAPP.Models
{
    [Table("CampuriTabele")]
    public class CampTabel
    {
        [Key]
        public int id_coloana { get; set; }

        [Required]
        [MaxLength(50)]
        public string nume_tabel { get; set; }

        [Required]
        [MaxLength(50)]
        public string nume_coloana { get; set; }
    }
}
