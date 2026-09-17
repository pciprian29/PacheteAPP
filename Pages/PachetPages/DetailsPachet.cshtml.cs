using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Models;
using PacheteAPP.Models.Views;
using PacheteAPP.Data;
using Microsoft.AspNetCore.Authorization;

namespace PacheteAPP.Pages.PachetPages;

[Authorize(Policy = "CanView")]
public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public VwPacheteComplet Pachet { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id_pachet)
    {
        if (id_pachet is null)
        {
            return NotFound();
        }

        var pachet = await _context.VwPacheteComplet.FindAsync(id_pachet);
        if (pachet is null)
        {
            return NotFound();
        }
        else
        {
            Pachet = pachet;
        }
        return Page();
    }

}