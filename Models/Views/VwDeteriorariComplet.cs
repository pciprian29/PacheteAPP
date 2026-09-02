using System;
using System.ComponentModel.DataAnnotations;

namespace PacheteAPP.Models.Views
{
    public class VwDeteriorariComplet
    {
        [Key]
        public int id_deteriorare { get; set; }
        public string locatie_deteriorare { get; set; }
        public string descriere_deteriorare { get; set; }
        public int? id_inregistrare { get; set; }
        public DateTime? data_inregistrare { get; set; }
        public string awb { get; set; }
        public string utilizator_inregistrare { get; set; }

    }
}
