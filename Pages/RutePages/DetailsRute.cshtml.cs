using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace PacheteAPP.Pages.RutePages
{
    [Authorize(Policy = "CanView")]
    public class DetailsRuteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsRuteModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public Ruta Ruta { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var ruta = await _context.Rute.Include(r => r.TipRuta).FirstOrDefaultAsync(m => m.id_ruta == id);

            if(ruta == null)
            {
                return NotFound();
            }

            Ruta = ruta;
            return Page();
        }
    }
}
