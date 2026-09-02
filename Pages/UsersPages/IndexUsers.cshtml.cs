using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models.Views;

namespace PacheteAPP.Pages.UsersPages
{
    [Authorize(Roles ="Admin")]
    public class IndexUsersModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexUsersModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<VwUseriRoluri> ListaUseri { get; set; } = default!;

        public async Task OnGetAsync()
        {
            ListaUseri = await _context.VwUseriRoluri.ToListAsync();
        }
    }
}
