using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;
using PacheteAPP.Models.Helper;

namespace PacheteAPP.Pages.UsersPages
{
    [Authorize(Roles ="Admin")]
    public class CreateUsersModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public CreateUsersModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputUser Input { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = new AppUser
            {
                UserName = Input.email,
                Email = Input.email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, Input.password);

            if (result.Succeeded)
            {
                return RedirectToPage("/UsersPages/IndexUsers");
            }

            return Page();
        }
    } 
}
