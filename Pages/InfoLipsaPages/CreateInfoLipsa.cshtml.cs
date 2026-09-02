using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace PacheteAPP.Pages.InfoLipsaPages
{
    [Authorize(Policy = "CanCreate")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }



        [BindProperty(SupportsGet = true)]
        public string PachetAwb { get; set; }

        [BindProperty]
         public InformatieLipsa InformatieLipsa { get; set; } = default!;
        public IActionResult OnGet(string awb)
        {
            if (!string.IsNullOrEmpty(awb))
            {
                PachetAwb = awb;
            }

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
        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var val = ModelState[key];
                    foreach (var error in val.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"===> EROARE la câmpul [{key}]: {error.ErrorMessage} <===");
                    }
                }
                return Page();
            }
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userIdInt = int.Parse(userIdString);

            var PachetId = await _context.Pachete
                .Where(p => p.awb == PachetAwb)
                .Select(p => p.id_pachet)
                .FirstOrDefaultAsync();
            if (PachetId == 0)
            {
                ModelState.AddModelError("PachetAwb", "AWB-ul nu exista. Te rugam sa folosesti sugestiile din lista");
                return Page();
            }
            var pachet = await _context.Pachete.FindAsync(PachetId);

            if (pachet != null)
            {
                string? numeStatusCurent = await _context.StatusPachet
                    .Where(s => s.id_status_pachet == pachet.id_status_pachet)
                    .Select(s => s.denumire)
                    .FirstOrDefaultAsync();

                int noulStatusId = pachet.id_status_pachet;

                if (numeStatusCurent == "Deteriorat")
                {
                    noulStatusId = 6;
                }
                else if (numeStatusCurent != "InformatiiLipsa" && numeStatusCurent != "Deteriorat+InfoLipsa")
                {
                    noulStatusId = 5;
                }

                if (pachet.id_status_pachet != noulStatusId)
                {
                    pachet.id_status_pachet = noulStatusId;
                }
            }
            var inregistrareNoua = new Inregistrare
            {
                id_pachet = PachetId,
                id_tip_inregistrare = 2,
                id_user = userIdInt,
            };

            _context.Inregistrari.Add(inregistrareNoua);
            await _context.SaveChangesAsync();

            InformatieLipsa.id_inregistrare = inregistrareNoua.id_inregistrare;
            _context.InformatiiLipsa.Add(InformatieLipsa);
            await _context.SaveChangesAsync();

            return RedirectToPage("./IndexInfoLipsa");
        }
    }
}
