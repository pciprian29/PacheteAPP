using System.ComponentModel.DataAnnotations;

namespace PacheteAPP.Models.Views
{
    public class VwIstoricModificari
    {
        [Key]
        public int id_modificare { get; set; }
        public string? valoare_noua { get; set; }
        public string valoare_veche { get; set; }
        public string descriere { get; set; }
        public DateTime? data_modificare { get; set; }
        public string awb_pachet { get; set; }
        public int user_modificare { get; set;}
        public int id_pachet { get; set; }
        public string UserName { get; set; }

    }
}
