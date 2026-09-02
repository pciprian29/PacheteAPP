using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PacheteAPP.Models.Types;

[Table("StatusPachet")]
public class StatusPachet
{
    [Key]
    public int id_status_pachet { get; set; }
    [Required]
    public string denumire { get; set; }
}
