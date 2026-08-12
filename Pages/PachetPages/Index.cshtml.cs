using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Models;
using PacheteAPP.Data;

namespace PacheteAPP.Pages.PachetPages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Pachet> Pachet { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Pachet = await _context.Pachete.ToListAsync();
    }
}
