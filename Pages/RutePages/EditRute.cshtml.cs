using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;


namespace PacheteAPP.Pages.RutePages
{
    [Authorize(Policy = "CanEdit")]
    public class EditRuteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditRuteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ruta Ruta { get; set; } = default!;

        public SelectList tipuriRute { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Rute == null)
            {
                return NotFound();
            }

            var ruta = await _context.Rute.FirstOrDefaultAsync(m => m.id_ruta == id);

            if (ruta == null)
            {
                return NotFound();
            }

            Ruta = ruta;
            tipuriRute = new SelectList(_context.TipuriRute, "id_tip_ruta", "denumire");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Ruta.TipRuta");
            ModelState.Remove("Ruta.Transporturi");

            if (!ModelState.IsValid)
            {
                tipuriRute = new SelectList(_context.TipuriRute, "id_tip_ruta", "denumire");
                return Page();
            }

            _context.Rute.Update(Ruta);
            await _context.SaveChangesAsync();

            return RedirectToPage("./IndexRute");
        }
    }
}
