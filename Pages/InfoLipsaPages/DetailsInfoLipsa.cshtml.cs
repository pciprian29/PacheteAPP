using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;
using PacheteAPP.Models.Views;
using Microsoft.AspNetCore.Authorization;

namespace PacheteAPP.Pages.InfoLipsaPages;

[Authorize(Policy = "CanView")]
public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public VwInfoLipsaComplet InfoLipsa { get; set; } = default!;

    public int idPachet { get; set; }

    public List<Imagine> Imagini { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id_info_lipsa)
    {
        if(id_info_lipsa == null)
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

        if(InfoLipsa != null && !string.IsNullOrEmpty(InfoLipsa.awb))
        {
            var pachet = await _context.Pachete.AsNoTracking().FirstOrDefaultAsync(p => p.awb == InfoLipsa.awb);
            if(pachet != null)
            {
                idPachet = pachet.id_pachet;
            }
        }

        Imagini = await _context.Imagini
            .Where(i => i.id_tip_imagine == 1 && i.id_entitate == id_info_lipsa)
            .OrderBy(i => i.data_creare)
            .ToListAsync();

        return Page();

    }

   
}
