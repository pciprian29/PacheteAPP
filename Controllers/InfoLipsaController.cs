using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PacheteAPP.DTOs.CreateInfoLipsa;
using PacheteAPP.Services.CreateInfoLipsa;
using System.Security.Claims;

namespace PacheteAPP.Controllers
{
    [Route("api/android/infolipsa")]
    [ApiController]
    [Authorize(Policy ="CanCreate")]
    public class InfoLipsaController(IInfoLipsaService infoLipsaService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInfoLipsaRequest request, CancellationToken ct)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString is null || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var result = await infoLipsaService.CreateAsync(request, userId, ct);

            if(result.Success)
            {
                return CreatedAtAction(nameof(Create), new { id = result.idInfoLipsa }, result);
            }

            return ValidationProblem(new ValidationProblemDetails(
                 result.Errors.ToDictionary(e => e.Key, e => new[] { e.Value })));
        }
    }
}
