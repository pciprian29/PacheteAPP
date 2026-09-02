using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;
using PacheteAPP.Models.Views;
using Microsoft.AspNetCore.Authorization;

namespace PacheteAPP.Pages.InfoLipsaPages
{
    [Authorize(Policy = "CanDelete")]
    public class DeleteInfoLipsaModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteInfoLipsaModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public VwInfoLipsaComplet InfoLipsa { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id_info_lipsa)
        {
            if (id_info_lipsa == null)
            {
                return NotFound();
            }
            var infoLipsa = await _context.VwInfoLipsaComplet.FirstOrDefaultAsync(m => m.id_info_lipsa == id_info_lipsa);
            if (infoLipsa == null)
            {
                return NotFound();
            }
            else
            {
                InfoLipsa = infoLipsa;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id_info_lipsa)
        {
            if (id_info_lipsa == null)
            {
                return NotFound();
            }
            var infoLipsa = await _context.InformatiiLipsa.FindAsync(id_info_lipsa);
            var inregistrare = await _context.Inregistrari.FirstOrDefaultAsync(i => i.id_inregistrare == infoLipsa.id_inregistrare);
            if (infoLipsa == null)
            {
                return NotFound();

            }
            else
            {
                _context.Remove(infoLipsa);
                await _context.SaveChangesAsync();

                _context.Remove(inregistrare);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("/InfoLipsaPages/IndexInfoLipsa");
        }
    }
}
