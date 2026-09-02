using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;

namespace PacheteAPP.Pages.RutePages
{
    [Authorize(Policy = "CanView")]
    public class IndexRuteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexRuteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Ruta> Rute { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Rute != null)
            {
                Rute = await _context.Rute
                    .Include(r => r.TipRuta)
                    .ToListAsync();
            }
        }
    }
}
