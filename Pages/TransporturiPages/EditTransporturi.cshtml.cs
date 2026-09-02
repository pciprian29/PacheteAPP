using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;

namespace PacheteAPP.Pages.TransporturiPages
{
    [Authorize(Policy = "CanEdit")]
    public class EditTransporturiModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditTransporturiModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Transport Transport { get; set; } = default!;

        public SelectList StatusList { get; set; } = default!;

        public string NumeRutaCurenta { get; set; }
        public string UsernameCurent { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var transport = await _context.Transporturi
                .Include(t => t.Ruta)
                .FirstOrDefaultAsync(m => m.id_transport == id);

            if (transport == null) return NotFound();

            Transport = transport;
            StatusList = new SelectList(_context.StatusTransporturi, "id_status_transport", "denumire");

            if (Transport.Ruta != null)
            {
                NumeRutaCurenta = Transport.Ruta.cod_ruta;
            }

            var user = await _context.Users.FindAsync(Transport.id_user);
            if (user != null)
            {
                UsernameCurent = user.UserName;
            }

            return Page();
        }

        public async Task<JsonResult> OnGetCautareRuteAsync(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return new JsonResult(new List<object>());

            var ruteGasite = await _context.Rute
                .Where(r => r.cod_ruta.Contains(q))
                .Select(r => new { id_ruta = r.id_ruta, cod_ruta = r.cod_ruta })
                .Take(10)
                .ToListAsync();

            return new JsonResult(ruteGasite);
        }

        public async Task<JsonResult> OnGetCautareUseriAsync(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return new JsonResult(new List<object>());

            var useriGasiti = await _context.Users
                .Where(u => u.UserName.Contains(q))
                .Select(u => new { id_user = u.Id, username = u.UserName })
                .Take(10)
                .ToListAsync();

            return new JsonResult(useriGasiti);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Transport.Ruta");
            ModelState.Remove("Transport.PachetePeTransport");

            if (!ModelState.IsValid)
            {
                StatusList = new SelectList(_context.StatusTransporturi, "id_status_transport", "denumire");
                return Page();
            }

            _context.Attach(Transport).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransportExists(Transport.id_transport))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./IndexTransporturi");
        }

        private bool TransportExists(int id)
        {
            return _context.Transporturi.Any(e => e.id_transport == id);
        }
    }
}