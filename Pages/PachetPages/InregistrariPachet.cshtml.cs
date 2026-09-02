using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Models.Views;
using PacheteAPP.Models.Types;
using PacheteAPP.Data;
using Microsoft.AspNetCore.Authorization;

namespace PacheteAPP.Pages.PachetPages
{
    [Authorize(Policy = "CanView")]
    public class InregistrariModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public InregistrariModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<VwDeteriorariComplet> Deteriorare { get; set; } = default!;
        public IList<VwInfoLipsaComplet> InfoLipsa { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string awbPachet)
        {
            var existaPachet = await _context.Pachete
                .AnyAsync(p => p.awb == awbPachet);

            if (!existaPachet)
            {
                return NotFound();
            }

            Deteriorare = await _context.VwDeteriorariComplet
                .Where(d => d.awb == awbPachet)
                .ToListAsync();

            InfoLipsa = await _context.VwInfoLipsaComplet
                .Where(i => i.awb == awbPachet)
                .ToListAsync();

            return Page();
        }
    }
}