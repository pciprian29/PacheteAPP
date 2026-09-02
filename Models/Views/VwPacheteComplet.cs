using System;
using System.ComponentModel.DataAnnotations;

namespace PacheteAPP.Models
{
    public class VwPacheteComplet
    {
        [Key]
        public int id_pachet { get; set; }
        public string awb { get; set; }
        public string descriere_pachet { get; set; }
        public string adresa_expeditor { get; set; }
        public string adresa_destinatar { get; set; }
        public decimal greutate_teoretica { get; set; }
        public decimal greutate_efectiva { get; set; }
        public int id_tip_pachet { get; set; }
        public string tip_pachet { get; set; }
        public int id_status_pachet { get; set; }
        public string status_pachet { get; set; }
        public string email_expeditor { get; set; }
        public string nume_complet_expeditor { get; set; }
        public string email_destinatar { get; set; }
        public string nume_complet_destinatar { get; set; }
        public DateTime? data_inregistrare { get; set; }
        public string UserName { get; set; }

        public int numar_deteriorari { get; set; }

        public int numar_infolipsa { get; set; }
    }
}