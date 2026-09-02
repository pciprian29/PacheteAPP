using System.ComponentModel.DataAnnotations;

namespace PacheteAPP.Models.Types
{
    public class TipRuta
    {
        [Key]
        public int id_tip_ruta { get; set; }
        public string denumire { get; set; }
    }
}
