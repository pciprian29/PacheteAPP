using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Models;
using PacheteAPP.Data;
using PacheteAPP.Models.Helper;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace PacheteAPP.Pages.PachetPages;

[Authorize(Policy = "CanCreate")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<JsonResult> OnGetCautareEmailAsync(string q)
    {
        if(string.IsNullOrWhiteSpace(q))
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

    [BindProperty]
    public Pachet Pachet { get; set; } = default!;

    //radio
    [BindProperty]
    public string TipExpeditor { get; set; } = "existent";

    [BindProperty]
    public string TipDestinatar { get; set; } = "existent";

 
    [BindProperty]
    public string? ExpeditorNume { get; set; }

    [BindProperty]
    public string? ExpeditorPrenume { get; set; }

    [BindProperty]
    public string? DestinatarNume { get; set; }

    [BindProperty]
    public string? DestinatarPrenume { get; set; }

    [BindProperty]
    public bool ConfirmaSuprascriereExpeditor { get; set; }

    [BindProperty]
    public bool ConfirmaSuprascriereDestinatar { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            foreach (var key in ModelState.Keys)
            {
                var val = ModelState[key];
                foreach (var error in val.Errors)
                {
                    System.Diagnostics.Debug.WriteLine($"===> EROARE la campul [{key}]: {error.ErrorMessage} <===");
                }
            }
            return Page();
        }

        if (!Regex.IsMatch(Pachet.email_expeditor, @"(?<![a-zA-Z@.-])[a-zA-Z0-9]+@[a-zA-Z]+\.[a-z]+(?![A-Za-z@.-])"))
        {
            ModelState.AddModelError("Pachet.email_expeditor", "Formatul emailului expeditor este invalid");
            return Page();
        }

        if(!Regex.IsMatch(Pachet.email_destinatar, @"(?<![a-zA-Z@.-])[a-zA-Z0-9]+@[a-zA-Z]+\.[a-z]+(?![A-Za-z@.-])"))
        {
            ModelState.AddModelError("Pachet.email_destinatar", "Formatul emailului destinatar este invalid");
            return Page();
        }

        if (TipDestinatar == "nou" && (ExpeditorNume is null or "" || ExpeditorPrenume is null or ""))
        {
            ModelState.AddModelError("ExpeditorNume", "Nume si prenume expeditor incomplete");
            ModelState.AddModelError("ExpeditorPrenume", "Nume si prenume expeditor incomplete");
            return Page();
        }

        if (TipDestinatar == "nou" && (DestinatarNume is null or "" || DestinatarPrenume is null or ""))
        {
            ModelState.AddModelError("DestinatarNume", "Nume si prenume destinatar incomplete");
            ModelState.AddModelError("DestinatarPrenume", "Nume si prenume destinatar incomplete");
            return Page();
        }

        bool necesitaConfirmare = false;

        // EXPEDITOR 
        if (TipExpeditor == "existent")
        {
            var userExistent = await _context.Clienti.FindAsync(Pachet.email_expeditor);
            if (userExistent == null)
            {
                ModelState.AddModelError("Pachet.email_expeditor", "Acest email nu exista in baza de date! Bifati casuta de mai jos pentru adaugarea unui client nou!");
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
                    ModelState.AddModelError("Pachet.email_expeditor", "Acest email exista deja! Bifati castua de confirmare de mai jos pentru a-i edita datele.");
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

        // DESTINATAR

        if(TipDestinatar == "existent")
        {
            var userExistent = await _context.Clienti.FindAsync(Pachet.email_destinatar);
            if(userExistent == null)
            {
                ModelState.AddModelError("Pachet.email_expeditor", "Acest mail nu exista in baza de date");
                ViewData["EmailInexistentDestinatar"] = true;
                necesitaConfirmare = true;
            }
        }else if (TipDestinatar == "nou")
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
                    ModelState.AddModelError("Pachet.email_destinatar", "Acest email este deja inregistrat! Bifati caasuta pentru editarea clientului!");
                    ViewData["AvertizareDestinatar"] = true;
                    necesitaConfirmare = true;
                }
                else
                {
                    userExistent.nume = DestinatarNume;
                    userExistent.prenume = DestinatarPrenume;
                }
            }
        }

        if (necesitaConfirmare)
        {
            return Page();
        }

        Pachet.awb = "AWB_TEMPORAR";

        await _context.SaveChangesAsync();

        _context.Pachete.Add(Pachet);

        await _context.SaveChangesAsync();

        string awbGenerat = GeneratorAwb.GenerareAwb(Pachet.id_pachet);
        
        Pachet.awb = awbGenerat;

        _context.Pachete.Update(Pachet);
        await _context.SaveChangesAsync();

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdString != null)
        {
            var userIdInt = int.Parse(userIdString);
            var inregistrareNoua = new Inregistrare
            {
                id_pachet = Pachet.id_pachet,
                id_tip_inregistrare = 1,
                id_user = userIdInt
            };

            _context.Inregistrari.Add(inregistrareNoua);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./IndexPachet");
    }
}