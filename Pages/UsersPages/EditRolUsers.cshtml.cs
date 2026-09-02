using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using PacheteAPP.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PacheteAPP.Pages.UsersPages
{
    [Authorize(Roles = "Admin")]
    public class EditRolUsersModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public EditRolUsersModel(UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public int idUser { get; set; }

        public string UserName { get; set; }

        [BindProperty]
        public string numeRolSelectat { get; set; }

        public SelectList listaRoluri { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id.Value.ToString());
            if (user == null) return NotFound();

            idUser = user.Id;
            UserName = user.UserName;

            var roluriUser = await _userManager.GetRolesAsync(user);
            var rolCurent = roluriUser.FirstOrDefault();

            var allRoles = _roleManager.Roles.ToList();
            listaRoluri = new SelectList(allRoles, "Name", "Name", rolCurent);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(idUser.ToString());
            if (user == null) return NotFound();

            var roluriCurente = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roluriCurente);

            if (!string.IsNullOrEmpty(numeRolSelectat))
            {
                await _userManager.AddToRoleAsync(user, numeRolSelectat);
            }

          return RedirectToPage("./IndexUsers");
        }
    }
}