using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PacheteAPP.Models
{
    [Table("Clienti")]
    public class Client
    {
        [Key]
        public string email_client { get; set; }
        [Required]
        public string nume { get; set; }
        [Required]
        public string prenume { get; set; }

        [Required]
        public bool expeditor_destinatar { get; set; }

    }
}
