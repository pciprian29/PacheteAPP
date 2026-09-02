using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;

namespace PacheteAPP.Pages.PachetPages
{
    [Authorize(Policy = "CanEdit")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Pachet Pachet { get; set; } = new Pachet();
        
        [BindProperty] public string TipExpeditor { get; set; } = "existent";
        [BindProperty] public string TipDestinatar { get; set; } = "existent";

        [BindProperty] public string? ExpeditorNume { get; set; }
        [BindProperty] public string? ExpeditorPrenume { get; set; }
        [BindProperty] public string? DestinatarNume { get; set; }
        [BindProperty] public string? DestinatarPrenume { get; set; }

        [BindProperty] public bool ConfirmaSuprascriereExpeditor { get; set; }
        [BindProperty] public bool ConfirmaSuprascriereDestinatar { get; set; }

        // tom select ajax
        public async Task<JsonResult> OnGetCautareEmailAsync(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return new JsonResult(new List<object>());
            }
            var emailuriGasite = await _context.Clienti
                .Where(c => c.email_client.Contains(q))
                .Select(c => new { email = c.email_client })
                .Take(10)
                .ToListAsync();
            return new JsonResult(emailuriGasite);
        }

        public async Task<IActionResult> OnGetAsync(int? id_pachet)
        {
            if (id_pachet == null) return NotFound();

            var pachet = await _context.Pachete.FirstOrDefaultAsync(m => m.id_pachet == id_pachet);
            if (pachet == null) return NotFound();

            Pachet = pachet;

            IncarcaDropdownuri();
            int cucu = 2;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Daca formularul e invalid sau Pachet a "disparut"
            if (!ModelState.IsValid || Pachet == null)
            {
                // Ne asiguram ca Pachet nu e null ca sa nu crape pagina HTML
                if (Pachet == null)
                {
                    Pachet = new Pachet();
                }

                IncarcaDropdownuri();
                return Page(); // Reincarcam pagina cu mesajele de eroare
            }
            bool necesitaConfirmare = false;

            //expeditor
            if (TipExpeditor == "existent")
            {
                var userExistent = await _context.Clienti.FindAsync(Pachet.email_expeditor);
                if (userExistent == null)
                {
                    ModelState.AddModelError("Pachet.email_expeditor", "Acest email nu exista in baza de date! Bifati casuta de mai jos pentru a-l adauga.");
                    ViewData["EmailInexistentExpeditor"] = true;
                    necesitaConfirmare = true;
                }
            }
            else if (TipExpeditor == "nou")
            {
                var userExistent = await _context.Clienti.FindAsync(Pachet.email_expeditor);
                if (userExistent == null)
                {
                    _context.Clienti.Add(new Client
                    {
                        email_client = Pachet.email_expeditor,
                        nume = ExpeditorNume,
                        prenume = ExpeditorPrenume,
                        expeditor_destinatar = false
                    });
                }
                else
                {
                    if (!ConfirmaSuprascriereExpeditor)
                    {
                        ModelState.AddModelError("Pachet.email_expeditor", "Acest email există deja! Bifati căsuța de confirmare de mai jos pentru a-i suprascrie datele.");
                        ViewData["AvertizareExpeditor"] = true;
                        necesitaConfirmare = true;
                    }
                    else
                    {
                        userExistent.nume = ExpeditorNume;
                        userExistent.prenume = ExpeditorPrenume;
                    }
                }
            }
            // destinatar 
            if (TipDestinatar == "existent")
            {
                var userExistent = await _context.Clienti.FindAsync(Pachet.email_destinatar);
                if (userExistent == null)
                {
                    ModelState.AddModelError("Pachet.email_destinatar", "Acest email nu exista în baza de date! Bifati casuta de mai jos pentru a-l adauga.");
                    ViewData["EmailInexistentDestinatar"] = true;
                    necesitaConfirmare = true;
                }
            }
            else if (TipDestinatar == "nou")
            {
                var userExistent = await _context.Clienti.FindAsync(Pachet.email_destinatar);
                if (userExistent == null)
                {
                    _context.Clienti.Add(new Client
                    {
                        email_client = Pachet.email_destinatar,
                        nume = DestinatarNume,
                        prenume = DestinatarPrenume,
                        expeditor_destinatar = true
                    });
                }
                else
                {
                    if (!ConfirmaSuprascriereDestinatar)
                    {
                        ModelState.AddModelError("Pachet.email_destinatar", "Acest email exista deja! Bifati căsuța de confirmare de mai jos pentru a-i suprascrie datele.");
                        ViewData["AvertizareDestinatar"] = true;
                        necesitaConfirmare = true;
                    }
                    else
                    {
                        userExistent.nume = DestinatarNume;
                        userExistent.prenume = DestinatarPrenume;
                        userExistent.expeditor_destinatar = true;
                    }
                }
            }

            if (necesitaConfirmare)
            {
                IncarcaDropdownuri();
                return Page();
            }


            var pachetDeModificat = await _context.Pachete.FindAsync(Pachet.id_pachet);
            if (pachetDeModificat == null) return NotFound();

            pachetDeModificat.id_tip_pachet = Pachet.id_tip_pachet;
            pachetDeModificat.descriere = Pachet.descriere;

            pachetDeModificat.email_expeditor = Pachet.email_expeditor;
            pachetDeModificat.adresa_expeditor = Pachet.adresa_expeditor;

            pachetDeModificat.email_destinatar = Pachet.email_destinatar;
            pachetDeModificat.adresa_destinatar = Pachet.adresa_destinatar;

            pachetDeModificat.greutate_teoretica = Pachet.greutate_teoretica;
            pachetDeModificat.greutate_efectiva = Pachet.greutate_efectiva;
            pachetDeModificat.id_status_pachet = Pachet.id_status_pachet;

            await _context.SaveChangesAsync();

            return RedirectToPage("./DetailsPachet", new { id_pachet = Pachet.id_pachet });
        }

        private void IncarcaDropdownuri()
        {
            ViewData["id_tip_pachet"] = new SelectList(_context.TipuriPachete, "id_tip_pachet", "denumire", Pachet.id_tip_pachet);
            ViewData["id_status_pachet"] = new SelectList(_context.StatusPachet, "id_status_pachet", "denumire", Pachet.id_status_pachet);
        }
    }
}