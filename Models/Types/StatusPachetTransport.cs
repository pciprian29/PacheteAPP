using System.ComponentModel.DataAnnotations;

namespace PacheteAPP.Models.Types
{
    public class StatusPachetTransport
    {
        [Key]
        public int id_status_pachet_transport { get; set; }
        public string denumire { get; set; }
    }
}
