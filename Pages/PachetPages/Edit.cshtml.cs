using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Models;
using PacheteAPP.Data;

namespace PacheteAPP.Pages.PachetPages;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
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
        Pachet = pachet;
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Pachet).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PachetExists(Pachet.id_pachet))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool PachetExists(int id_pachet)
    {
        return _context.Pachete.Any(e => e.id_pachet == id_pachet);
    }
}
