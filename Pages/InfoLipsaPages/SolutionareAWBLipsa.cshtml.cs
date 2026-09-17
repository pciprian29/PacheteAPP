using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;
using PacheteAPP.Models.Views;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace PacheteAPP.Pages.InfoLipsaPages
{
    [Authorize(Policy = "CanEdit")]
    public class SolutionareAWBLipsaModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SolutionareAWBLipsaModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public VwInfoLipsaComplet InfoLipsa { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string? FiltruDescriere { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? FiltruAwb { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? FiltruGreutateTeoretica { get; set; }
        [BindProperty(SupportsGet = true)]
        public decimal? FiltruGreutateEfectiva { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FiltruEmailExpeditor { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? FiltruEmailDestinatar { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? FiltruAdresaExpeditor { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? FiltruAdresaDestinatar { get; set; }


        public List<Pachet> Rezultate { get; set; } = new();
        public bool AuCautat { get; set; }

        [BindProperty]
        public int PachetSelectatId { get; set; }

        public List<Imagine> Imagini { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var infoLipsa = await _context.VwInfoLipsaComplet
                .FirstOrDefaultAsync(v => v.id_info_lipsa == Id);

            if (infoLipsa == null)
            {
                return NotFound();
            }

            InfoLipsa = infoLipsa;

            AuCautat = FiltreCompletate();
            if (AuCautat)
            {
                Rezultate = await CautaPachete();
            }

            Imagini = await _context.Imagini
                .Where(i => i.id_tip_imagine == 1 && i.id_entitate == Id)
                .OrderBy(i => i.data_creare)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var infoLipsa = await _context.InformatiiLipsa.FindAsync(Id);
            if (infoLipsa == null)
            {
                return NotFound();
            }

            var pachet = await _context.Pachete.FindAsync(PachetSelectatId);
            if (pachet == null)
            {
                ModelState.AddModelError(string.Empty, "Pachetul selectat nu mai exista");
                var infoLipsaView = await _context.VwInfoLipsaComplet.FirstOrDefaultAsync(v => v.id_info_lipsa == Id);
                InfoLipsa = infoLipsaView!;
                return Page();
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userIdInt = int.Parse(userIdString);

            var inregistrareNoua = new Inregistrare
            {
                id_pachet = PachetSelectatId,
                id_tip_inregistrare = 2,
                id_user = userIdInt,
            };

            _context.Inregistrari.Add(inregistrareNoua);
            await _context.SaveChangesAsync();

            infoLipsa.id_inregistrare = inregistrareNoua.id_inregistrare;
            _context.InformatiiLipsa.Update(infoLipsa);
            await _context.SaveChangesAsync();

            return RedirectToPage("./IndexInfoLipsa");
        }

        private bool FiltreCompletate()
        {
            return !string.IsNullOrWhiteSpace(FiltruEmailExpeditor)
                || !string.IsNullOrWhiteSpace(FiltruEmailDestinatar)
                || !string.IsNullOrWhiteSpace(FiltruAdresaExpeditor)
                || !string.IsNullOrWhiteSpace(FiltruAdresaDestinatar)
                || !string.IsNullOrWhiteSpace(FiltruDescriere)
                || !string.IsNullOrWhiteSpace(FiltruAwb)
                || (FiltruGreutateTeoretica != null)
                || (FiltruGreutateEfectiva != null);
        }

        private async Task<List<Pachet>> CautaPachete()
        {
            var query = _context.Pachete.AsQueryable();

            if (!string.IsNullOrWhiteSpace(FiltruEmailExpeditor))
                query = query.Where(p => p.email_expeditor.Contains(FiltruEmailExpeditor));

            if (!string.IsNullOrWhiteSpace(FiltruEmailDestinatar))
                query = query.Where(p => p.email_destinatar.Contains(FiltruEmailDestinatar));

            if (!string.IsNullOrWhiteSpace(FiltruAdresaExpeditor))
                query = query.Where(p => p.adresa_expeditor.Contains(FiltruAdresaExpeditor));

            if (!string.IsNullOrWhiteSpace(FiltruAdresaDestinatar))
                query = query.Where(p => p.adresa_destinatar.Contains(FiltruAdresaDestinatar));

            if (!string.IsNullOrWhiteSpace(FiltruDescriere))
                query = query.Where(p => p.descriere.Contains(FiltruDescriere));

            if (!string.IsNullOrWhiteSpace(FiltruAwb))
                query = query.Where(p => p.awb != null && p.awb.Contains(FiltruAwb));

            if (FiltruGreutateTeoretica != null)
                query = query.Where(p => p.greutate_teoretica == FiltruGreutateTeoretica);

            if (FiltruGreutateEfectiva != null)
                query = query.Where(p => p.greutate_teoretica == FiltruGreutateEfectiva);

            return await query.Take(26).ToListAsync();
        }
    }
}