using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;
using PacheteAPP.Models.Views;

namespace PacheteAPP.Pages.DeteriorariPages
{
    [Authorize(Policy = "CanDelete")]
    public class DeleteDeteriorareModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteDeteriorareModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public VwDeteriorariComplet Deteriorare { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync (int? id_deteriorare)
        {
            if(id_deteriorare == null)
            {
                return NotFound();
            }

            var deteriorare = await _context.VwDeteriorariComplet.FirstOrDefaultAsync(m => m.id_deteriorare == id_deteriorare);
            if(deteriorare == null)
            {
                return NotFound();
            }
            else
            {
                Deteriorare = deteriorare;
            }
            return Page();

        }
        public async Task<IActionResult> OnPostAsync(int? id_deteriorare)
        {
            if(id_deteriorare == null)
            {
                return NotFound();
            }
            var deteriorare = await _context.Deteriorari.FindAsync(id_deteriorare);
            var inregistrareDeteriorare = await _context.Inregistrari.FirstOrDefaultAsync(i => i.id_inregistrare == deteriorare.id_inregistrare);

            if(deteriorare == null)
            {
                return NotFound();
            }
            else
            {
                _context.Remove(deteriorare);
                _context.Remove(inregistrareDeteriorare);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/DeteriorariPages/IndexDeteriorare"); 
        }

    }
}
