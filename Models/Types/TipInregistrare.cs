using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PacheteAPP.Models.Types
{
    [Table("TipInregistrare")]
    public class TipInregistrare
    {
        [Key]
        public int id_tip_inregistrare { get; set; }
        [Required]
        public string denumire { get; set; }
    }
}
