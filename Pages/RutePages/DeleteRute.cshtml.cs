using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;

namespace PacheteAPP.Pages.RutePages
{
    [Authorize(Policy = "CanDelete")]
    public class DeleteRuteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteRuteModel(ApplicationDbContext context)
        {
            _context = context;
        }
        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var ruta = await _context.Rute.FindAsync(id);

            if(ruta != null)
            {
                Ruta = ruta;
                _context.Rute.Remove(Ruta);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./IndexRute");

        }


    }
}
