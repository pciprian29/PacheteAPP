using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using PacheteAPP.DTOs.Login;
using PacheteAPP.Services.Login;

namespace PacheteAPP.Controllers
{
    [Route("api/android")]
    [ApiController]
    public sealed class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AndroidLoginRequest request, CancellationToken ct)
        {
            var result = await authService.LoginAsync(request, ct);

            if (result.Success)
                return Ok(result);

            return Unauthorized(result.Errors);
        }
    }
}