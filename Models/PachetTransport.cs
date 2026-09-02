using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PacheteAPP.Models
{
    public class PachetTransport
    {
        [Key]
        public int id_legatura { get; set; }
        public int id_pachet { get; set; }
        [ForeignKey("id_pachet")]
        public Pachet Pachet { get; set; }
        public int id_transport { get; set; }
        [ForeignKey("id_transport")]
        public Transport Transport { get; set; }
        public int id_status_pachet_transport { get; set; }
    }
}
