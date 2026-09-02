using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;

namespace PacheteAPP.Pages.TransporturiPages
{
    [Authorize(Policy = "CanDelete")]
    public class DeleteTransporturiModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteTransporturiModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Transport Transport { get; set; } = default!;
        public string UsernameCurent { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var transport = await _context.Transporturi
                .Include(t => t.Ruta)
                .FirstOrDefaultAsync(m => m.id_transport == id);

            if (transport == null) return NotFound();

            Transport = transport;

            var user = await _context.Users.FindAsync(Transport.id_user);
            if (user != null)
            {
                UsernameCurent = user.UserName;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var transport = await _context.Transporturi.FindAsync(id);

            if (transport != null)
            {
                Transport = transport;
                _context.Transporturi.Remove(Transport);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./IndexTransporturi");
        }
    }
}