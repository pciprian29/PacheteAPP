using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PacheteAPP.DTOs;
using PacheteAPP.DTOs.CreatePachet;
using PacheteAPP.Services.CreatePachet;
using System.Security.Claims;

namespace PacheteAPP.Controllers
{
    [Route("api/android/pachet")]
    [ApiController]
    [Authorize(Policy = "CanCreate")]
    public sealed class PacheteController(IPachetService pachetService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePachetRequest request, CancellationToken ct)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString is null || !int.TryParse(userIdString, out var UserId))
            {
                return Unauthorized();
            }

            var result = await pachetService.CreateAsync(request, UserId, ct);

            if (result.Success)
                return CreatedAtAction(nameof(Create), new { id = result.IdPachet },
                    new CreatePachetResponse(result.IdPachet!.Value, result.Awb!));

            if (result.NecesitaConfirmare)
                return Conflict(result.Errors);

            return ValidationProblem(new ValidationProblemDetails(
                result.Errors.ToDictionary(e => e.Key, e => new[] { e.Value })));
        }

        [HttpGet("{awb}")]
        [Authorize(Policy = "CanView")]
        public async Task<IActionResult> ObtineDetaliiComplete(string awb, CancellationToken ct)
        {
            var rezultat = await pachetService.ObtineDetaliiCompleteAsync(awb, ct);

            if (rezultat == null)
                return NotFound();

            return Ok(rezultat);
        }
    }
}