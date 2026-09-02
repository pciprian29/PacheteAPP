using System.ComponentModel.DataAnnotations;

namespace PacheteAPP.Models.Views
{
    public class VwUseriRoluri
    {
        [Key]
        public int id_user { get; set; }
        public string UserName { get; set; }
        public int? id_rol { get; set; }

        public string? nume_rol { get; set; }

    }
}
