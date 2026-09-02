using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PacheteAPP.Pages.TransporturiPages
{
    [Authorize(Policy = "CanCreate")]
    public class CreateTransporturiModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateTransporturiModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Transport Transport { get; set; } = default!;

        public SelectList StatusList { get; set; } = default!;

        public IActionResult OnGet()
        {
            StatusList = new SelectList(_context.StatusTransporturi, "id_status_transport", "denumire");
            Transport = new Transport { data_plecare = DateTime.Now };

            return Page();
        }

        public async Task<JsonResult> OnGetCautareRuteAsync(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return new JsonResult(new List<object>());
            }

            var ruteGasite = await _context.Rute
                .Where(r => r.cod_ruta.Contains(q))
                .Select(r => new { id_ruta = r.id_ruta, cod_ruta = r.cod_ruta })
                .Take(10)
                .ToListAsync();

            return new JsonResult(ruteGasite);
        }

        public async Task<JsonResult> OnGetCautareUseriAsync(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return new JsonResult(new List<object>());
            }

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

            _context.Transporturi.Add(Transport);
            await _context.SaveChangesAsync();

            return RedirectToPage("./AlocarePachete", new { id = Transport.id_transport });
        }
    }
}