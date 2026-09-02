using System.ComponentModel.DataAnnotations;

namespace PacheteAPP.Models.Types
{
    public class StatusTransport
    {
        [Key]
        public int id_status_transport { get; set; }
        public string denumire { get; set; }
    }
}
