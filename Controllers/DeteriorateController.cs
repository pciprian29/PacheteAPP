using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PacheteAPP.DTOs.CreateDeteriorare;
using PacheteAPP.Services.CreateDeteriorare;
using System.Security.Claims;

namespace PacheteAPP.Controllers
{
    [Route("api/android/deteriorare")]
    [ApiController]
    [Authorize(Policy ="CanCreate")]
    public class DeteriorateController(IDeteriorareService deteriorareService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDeteriorareRequest request, CancellationToken ct)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString is null || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var result = await deteriorareService.CreateAsync(request, userId, ct);

            if (result.Success)
            {
                return CreatedAtAction(nameof(Create), new { id = result.IdDeteriorare }, result);
            }
            return ValidationProblem(new ValidationProblemDetails(
                result.Errors.ToDictionary(e => e.Key, e => new[] { e.Value })));

        }
    }
}
