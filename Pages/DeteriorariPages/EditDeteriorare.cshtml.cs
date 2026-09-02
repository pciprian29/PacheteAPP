using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Models;
using PacheteAPP.Data;
using Microsoft.AspNetCore.Authorization;

namespace PacheteAPP.Pages.DeteriorariPages;

[Authorize(Policy = "CanEdit")]
public class EditDeteriorareModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditDeteriorareModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public string PachetAwb { get; set; } = string.Empty;

    [BindProperty]
    public Deteriorare Deteriorare { get; set; } = new Deteriorare();

    public async Task<IActionResult> OnGetAsync(int? id_deteriorare)
    {
        if(id_deteriorare == null)
        {
            return NotFound();
        }
        var deteriorare = await _context.Deteriorari.FindAsync(id_deteriorare);
        if(deteriorare == null)
        {
            return NotFound();
        }
        Deteriorare = deteriorare;

        var viewData = await _context.VwDeteriorariComplet
            .FirstOrDefaultAsync(v => v.id_deteriorare == id_deteriorare);

        if(viewData != null)
        {
            PachetAwb = viewData.awb;
        }
        else
        {
            return NotFound();
        }

        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var deteriorareDeModificat = await _context.Deteriorari.FindAsync(Deteriorare.id_deteriorare);
        if (deteriorareDeModificat == null)
        {
            return NotFound();
        }
        deteriorareDeModificat.locatie_deteriorare = Deteriorare.locatie_deteriorare;
        deteriorareDeModificat.descriere_deteriorare = Deteriorare.descriere_deteriorare;

        await _context.SaveChangesAsync();

        return RedirectToPage("/DeteriorariPages/IndexDeteriorare");

    }
        
}
