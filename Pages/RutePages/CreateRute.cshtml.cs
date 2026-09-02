using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PacheteAPP.Data;
using PacheteAPP.Models;


namespace PacheteAPP.Pages.RutePages
{
    [Authorize(Policy = "CanCreate")]
    public class CreateRuteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateRuteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            tipuriRute = new SelectList(_context.TipuriRute, "id_tip_ruta", "denumire");
            return Page();
        }

        [BindProperty]
        public Ruta Ruta { get; set; } = default!;

        public SelectList tipuriRute { get; set; } = default!;

        private string GetPrefixDinTip(string denumireTip)
        {
            if (string.IsNullOrEmpty(denumireTip)) return "RUT";

            string denumire = denumireTip.ToLower();
            if (denumire.Contains("colectare")) return "COL";
            if (denumire.Contains("livrare")) return "LIV";
            if (denumire.Contains("tranzit")) return "TRZ";
            if (denumire.Contains("retur")) return "RET";
            if (denumire.Contains("express")) return "EXP";

            return "RUT";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Ruta.cod_ruta");
            ModelState.Remove("Ruta.TipRuta");
            ModelState.Remove("Ruta.Transporturi");

            if (!ModelState.IsValid) 
            {
                tipuriRute = new SelectList(_context.TipuriRute, "id_tip_ruta", "denumire");
                return Page();
            }

            Ruta.cod_ruta = "TEMP";

            _context.Rute.Add(Ruta);
            await _context.SaveChangesAsync();

            var tipRuta = await _context.TipuriRute.FindAsync(Ruta.id_tip_ruta);
            string prefix = GetPrefixDinTip(tipRuta?.denumire);

            Ruta.cod_ruta = $"{prefix}-{Ruta.id_ruta:D4}";
            await _context.SaveChangesAsync();

            return RedirectToPage("/RutePages/IndexRute");
        }
    }
}
