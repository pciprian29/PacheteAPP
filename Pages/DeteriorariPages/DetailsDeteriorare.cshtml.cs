using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;
using PacheteAPP.Models.Views;

namespace PacheteAPP.Pages.DeteriorariPages
{
    [Authorize(Policy = "CanView")]
    public class DetailsDeteriorareModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsDeteriorareModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public VwDeteriorariComplet Deteriorare { get; set; } = default!;

        public int idPachet { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id_deteriorare)
        {
            if(id_deteriorare == null)
            {
                return NotFound();
            }

            var deteriorare = await _context.VwDeteriorariComplet.FirstOrDefaultAsync(m => m.id_deteriorare == id_deteriorare);
            if (deteriorare == null)
            {
                return NotFound();
            }
            Deteriorare = deteriorare;
            if (Deteriorare != null && !string.IsNullOrEmpty(Deteriorare.awb))
            {
                var pachet = await _context.Pachete
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.awb == Deteriorare.awb);

                if (pachet != null)
                {
                    idPachet = pachet.id_pachet;
                }
            }
            return Page();
        }
    }
}
