using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PacheteAPP.Models
{
    [Table("IstoricModificari")]
    public class IstoricModificari
    {
        [Key]
        public int id_modificare { get; set; }

        [Required]
        [MaxLength(255)]
        public string valoare_veche { get; set; }

        [Required]
        [MaxLength(255)]
        public string valoare_noua { get; set; }

        [Required]
        public DateTime data_modificare { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(255)]
        public string descriere { get; set; }

        [Required]
        [MaxLength(255)]
        public int user_modificare { get; set; }

        [Required]
        public int id_pachet { get; set; }

        [Required]
        public int id_coloana { get; set; }
    }
}
