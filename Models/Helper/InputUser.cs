using System.ComponentModel.DataAnnotations;

namespace PacheteAPP.Models.Helper
{
    public class InputUser
    {
        [Required]
        public string email { get; set; } = default!;
        [Required]
        public string password { get; set; } = default;
    }
}
