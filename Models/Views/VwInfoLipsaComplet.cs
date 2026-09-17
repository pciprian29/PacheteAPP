using System;
using System.ComponentModel.DataAnnotations;

namespace PacheteAPP.Models.Views
{
    public class VwInfoLipsaComplet
    {
        [Key]
        public int id_info_lipsa { get; set; }
        public string camp_afectat { get; set; }
        public string descriere_lipsa { get; set; }
        public string tip_lipsa { get; set; }
        public int? id_inregistrare { get; set; }
        public DateTime? data_inregistrare { get; set; }
        public string? tip_inregistrare { get; set; }
        public string? awb { get; set; }
        public string? utilizator_inregistrare { get; set; }
    }
}