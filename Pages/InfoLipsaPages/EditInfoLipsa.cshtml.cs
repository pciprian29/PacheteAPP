using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;
using Microsoft.AspNetCore.Authorization;

namespace PacheteAPP.Pages.InfoLipsaPages
{
    [Authorize(Policy = "CanEdit")]
    public class EditInfoLipsaModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditInfoLipsaModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InformatieLipsa InformatieLipsa { get; set; } = new InformatieLipsa();

        [BindProperty]
        public string PachetAwb { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int? id_info_lipsa)
        {
            if (id_info_lipsa == null)
            {
                return NotFound();
            }

            var infoLipsa = await _context.InformatiiLipsa.FindAsync(id_info_lipsa);
            if (infoLipsa == null)
            {
                return NotFound();
            }

            InformatieLipsa = infoLipsa;
            var viewData = await _context.VwInfoLipsaComplet
                .FirstOrDefaultAsync(v => v.id_info_lipsa == id_info_lipsa);

            if (viewData != null)
            {
                PachetAwb = viewData.awb;
            }

            IncarcaDropDown();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                IncarcaDropDown();
                return Page();
            }

            var infoDeModificat = await _context.InformatiiLipsa.FindAsync(InformatieLipsa.id_info_lipsa);

            if (infoDeModificat == null)
            {
                return NotFound();
            }

            infoDeModificat.camp_afectat = InformatieLipsa.camp_afectat;
            infoDeModificat.descriere = InformatieLipsa.descriere;

            infoDeModificat.id_tip_lipsa = InformatieLipsa.id_tip_lipsa;

            await _context.SaveChangesAsync();

            return RedirectToPage("./IndexInfoLipsa");
        }

        private void IncarcaDropDown()
        {
            ViewData["id_tip_lipsa"] = new SelectList(_context.TipuriLipsa, "id_tip_lipsa", "denumire", InformatieLipsa.id_tip_lipsa);
        }
    }
}