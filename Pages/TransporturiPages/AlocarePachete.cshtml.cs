using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;
using Microsoft.AspNetCore.Authorization;


namespace PacheteAPP.Pages.TransporturiPages
{
    [Authorize(Policy = "CanView")]
    public class AlocarePacheteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AlocarePacheteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Transport TransportCurent { get; set; } = default!;
        public IList<PachetTransport> PacheteDejaAlocate { get; set; } = default!;

        [BindProperty]
        public string AwbAdaugat { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();
            await IncarcaDatePagina(id.Value);
            return Page();
        }

        public async Task<JsonResult> OnGetCautareAwbAsync(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return new JsonResult(new List<object>());
            }

            var awburiGasite = await _context.Pachete
                .Where(p => p.awb.Contains(q))
                .Select(p => new { awb = p.awb })
                .Take(10)
                .ToListAsync();

            return new JsonResult(awburiGasite);
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await IncarcaDatePagina(id);

            if (string.IsNullOrWhiteSpace(AwbAdaugat))
            {
                TempData["Eroare"] = "Te rog sa introduci un AWB.";
                return Page();
            }

            var pachet = await _context.Pachete.FirstOrDefaultAsync(p => p.awb == AwbAdaugat);
            if (pachet == null)
            {
                TempData["Eroare"] = $"Pachetul cu AWB-ul {AwbAdaugat} nu exista in sistem!";
                return Page();
            }

            var existaDeja = await _context.Pachete_Transporturi
                .AnyAsync(pt => pt.id_transport == id && pt.id_pachet == pachet.id_pachet);

            if (existaDeja)
            {
                TempData["Eroare"] = "Acest pachet este deja in aceasta masina!";
                return Page();
            }

            var nouaAlocare = new PachetTransport
            {
                id_transport = id,
                id_pachet = pachet.id_pachet,
                id_status_pachet_transport = 1
            };

            _context.Pachete_Transporturi.Add(nouaAlocare);
            await _context.SaveChangesAsync();

            TempData["Succes"] = $"Pachetul {AwbAdaugat} a fost adaugat cu succes!";
            return RedirectToPage(new { id = id });
        }

        private async Task IncarcaDatePagina(int id)
        {
            TransportCurent = await _context.Transporturi
                .Include(t => t.Ruta)
                .FirstOrDefaultAsync(m => m.id_transport == id);

            PacheteDejaAlocate = await _context.Pachete_Transporturi
                .Include(pt => pt.Pachet)
                .Where(pt => pt.id_transport == id)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}