using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PacheteAPP.Models.Views
{
        public class VwTransporturiComplet
        {
            [Key]
            public int id_transport { get; set; }

            public DateTime data_plecare { get; set; }

            public DateTime? data_finalizare { get; set; } 

            public string numar_auto { get; set; }

            public string UserName { get; set; }

            public int id_status_transport { get; set; }

            public string status_transport { get; set; }

            public int id_ruta { get; set; }

            public string cod_ruta { get; set; }

            public string locatie_plecare { get; set; }

            public string locatie_destinatie { get; set; }

            public int numar_pachete { get; set; }
        }
}
