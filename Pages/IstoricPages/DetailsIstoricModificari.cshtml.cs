using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models.Views;

namespace PacheteAPP.Pages.IstoricPages
{
    [Authorize(Roles = "Admin")]
    public class DetailsIstoricModificariModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsIstoricModificariModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public VwIstoricModificari IstoricModificare { get; set; } = default;

        public async Task<IActionResult> OnGetAsync(int? id_modificare)
        {
            if(id_modificare == null)
            {
                return NotFound();
            }

            var istoricModificare = await _context.VwIstoricModificari.FirstOrDefaultAsync(m => m.id_modificare == id_modificare);
            if(istoricModificare == null)
            {
                return NotFound();
            }
            IstoricModificare = istoricModificare;
            return Page();
        }
    }
}
