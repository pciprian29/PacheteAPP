using PacheteAPP.Models.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PacheteAPP.Models
{
    [Table("Rute")]
    public class Ruta
    {
        [Key]
        public int id_ruta { get; set; }
        public string cod_ruta { get; set; }
        public int id_tip_ruta { get; set; }
        [ForeignKey("id_tip_ruta")]
        public TipRuta TipRuta { get; set; }
        public string locatie_plecare { get; set; }
        public string locatie_destinatie { get; set; }

        public ICollection<Transport> Transporturi { get; set; }
    }
}
