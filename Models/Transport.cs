using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PacheteAPP.Models
{
    [Table("Transporturi")]
    public class Transport
    {
        [Key]
        public int id_transport { get; set; }
        public int id_ruta { get; set; }
        [ForeignKey("id_ruta")]
        public Ruta Ruta { get; set; }

        public int id_user { get; set; }

        public string numar_auto { get; set; }
        public DateTime data_plecare { get; set; }
        public DateTime? data_finalizare { get; set; }
        public int id_status_transport { get; set; }

        public ICollection<PachetTransport> PachetePeTransport { get; set; }

    }
}
