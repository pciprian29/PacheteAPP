using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PacheteAPP.DTOs.Login;
using PacheteAPP.Models;

namespace PacheteAPP.Services.Login
{
    public sealed class AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole<int>> roleManager,
        IConfiguration config) : IAuthService
    {
        public async Task<LoginOperationResult> LoginAsync(AndroidLoginRequest request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.username) || string.IsNullOrWhiteSpace(request.password))
            {
                return LoginOperationResult.Fail("username", "Username și parola sunt obligatorii");
            }

            var user = await userManager.FindByNameAsync(request.username);
            if (user is null)
            {
                return LoginOperationResult.Fail("password", "Username sau parolă incorecte");
            }

            var checkResult = await signInManager.CheckPasswordSignInAsync(user, request.password, lockoutOnFailure: false);
            if (!checkResult.Succeeded)
            {
                return LoginOperationResult.Fail("password", "Username sau parolă incorecte");
            }

            var token = await GenerateJwtToken(user);
            return LoginOperationResult.Ok(token, user.UserName ?? request.username);
        }

        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var jwtSettings = config.GetSection("Jwt");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName ?? ""),
            };

            // rolurile userului
            var roleNames = await userManager.GetRolesAsync(user);

            foreach (var roleName in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));

                var role = await roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var roleClaims = await roleManager.GetClaimsAsync(role);
                    claims.AddRange(roleClaims);  
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}