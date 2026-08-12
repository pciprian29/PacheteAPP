using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Models;
using PacheteAPP.Data;

namespace PacheteAPP.Pages.PachetPages;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Pachet Pachet { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id_pachet)
    {
        if (id_pachet is null)
        {
            return NotFound();
        }

        var pachet = await _context.Pachete.FirstOrDefaultAsync(m => m.id_pachet == id_pachet);
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
