using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PacheteAPP.Models.Types
{
    [Table("TipPachet")]
    public class TipPachet
    {
        [Key]
        public int id_tip_pachet { get; set; }
        [Required]
        public string denumire { get; set; }
    }
}
