using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Models;
using PacheteAPP.Models.Views;
using PacheteAPP.Data;
using Microsoft.AspNetCore.Authorization;

namespace PacheteAPP.Pages.PachetPages;

[Authorize(Policy = "CanDelete")]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public VwPacheteComplet Pachet { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id_pachet)
    {
        if (id_pachet is null)
        {
            return NotFound();
        }

        var pachet = await _context.VwPacheteComplet.FirstOrDefaultAsync(m => m.id_pachet == id_pachet);
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

    public async Task<IActionResult> OnPostAsync(int? id_pachet)
    {
        if (id_pachet is null)
        {
            return NotFound();
        }

        var pachet = await _context.Pachete.FindAsync(id_pachet);
        if (pachet != null)
        {

            var inregistrari = await _context.Inregistrari
                .Where(i => i.id_pachet == id_pachet)
                .ToListAsync();

            if(inregistrari.Any())
            {
                var iduriInregistrari = inregistrari.Select(i => i.id_inregistrare).ToList();

                var deteriorari = await _context.Deteriorari
                    .Where(d => iduriInregistrari.Contains(d.id_inregistrare))
                    .ToListAsync();

                if (deteriorari.Any())
                {
                    _context.Deteriorari.RemoveRange(deteriorari);
                }

                var informatiilipsa = await _context.InformatiiLipsa
                    .Where(l => iduriInregistrari.Contains(l.id_inregistrare))
                    .ToListAsync();

                if (informatiilipsa.Any())
                {
                    _context.InformatiiLipsa.RemoveRange(informatiilipsa);
                }

                _context.Inregistrari.RemoveRange(inregistrari);
            }

            _context.Pachete.Remove(pachet);

            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./IndexPachet");
    }
}