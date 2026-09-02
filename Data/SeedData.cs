using Microsoft.AspNetCore.Identity;
using PacheteAPP.Models;
using System.Security.Claims;

namespace PacheteAPP.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();

            string adminRole = "Admin";
            string highUserRole = "HighUser";
            string lowUserRole = "LowUser";

            if(!await roleManager.RoleExistsAsync(adminRole))
            {
                var role = new IdentityRole<int>(adminRole);
                await roleManager.CreateAsync(role);

                await roleManager.AddClaimAsync(role, new Claim("Permisiune", "View"));
                await roleManager.AddClaimAsync(role, new Claim("Permisiune", "Create"));
                await roleManager.AddClaimAsync(role, new Claim("Permisiune", "Edit"));
                await roleManager.AddClaimAsync(role, new Claim("Permisiune", "Delete"));
            }

            if(!await roleManager.RoleExistsAsync(highUserRole))
            {
                var role = new IdentityRole<int>(highUserRole);
                await roleManager.CreateAsync(role);

                await roleManager.AddClaimAsync(role, new Claim("Permisiune", "View"));
                await roleManager.AddClaimAsync(role, new Claim("Permisiune", "Create"));
            }

            if(!await roleManager.RoleExistsAsync(lowUserRole))
            {
                var role = new IdentityRole<int>(lowUserRole);
                await roleManager.CreateAsync(role);

                await roleManager.AddClaimAsync(role, new Claim("Permisiune", "View"));
            }

            string emailAdmin = "pciprian2910@gmail.com";
            var user = await userManager.FindByEmailAsync(emailAdmin);

            if (user != null)
            {
                if (!await userManager.IsInRoleAsync(user, adminRole))
                {
                    await userManager.AddToRoleAsync(user, adminRole);
                }
            }

            string emailHighUser = "high_user@gmail.com";
            var highUser = await userManager.FindByEmailAsync(emailHighUser);

            if(highUser != null)
            {
                if(!await userManager.IsInRoleAsync(highUser, highUserRole))
                {
                    await userManager.AddToRoleAsync(highUser, highUserRole);
                }
            }

            string emailLowUser = "low_user@gmail.com";
                var lowUser = await userManager.FindByEmailAsync(emailLowUser);

            if(lowUser != null)
            {
                if(!await userManager.IsInRoleAsync(lowUser, lowUserRole))
                {
                    await userManager.AddToRoleAsync(lowUser, lowUserRole);
                }
            }
        }
    }
}
