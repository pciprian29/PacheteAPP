using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PacheteAPP.Data;
using PacheteAPP.DTOs.Imagini;
using PacheteAPP.Services.Imagini;
using System.Security.Claims;

namespace PacheteAPP.Controllers
{
    [ApiController]
    [Authorize(Policy = "CanCreate")]
    public class ImagineController(IImagineService imagineService): ControllerBase
    {
        [HttpPost("api/infolipsa/{idInfoLipsa:int}/imagini")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> UploadImagineInfoLipsa(
            int idInfoLipsa, [FromForm] UploadImagineRequest request, CancellationToken ct)
        {
            return await SalveazaImagine(request, idInfoLipsa, 1, ct);
        }

        [HttpPost("api/deteriorare/{idDeteriorare:int}/imagini")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> UploadImagineDeteriorare(
            int idDeteriorare, [FromForm] UploadImagineRequest request, CancellationToken ct)
        {
            return await SalveazaImagine(request, idDeteriorare, 2, ct);
        }

        private async Task<IActionResult> SalveazaImagine(UploadImagineRequest request, int idEntitate, int idTipImagine, CancellationToken ct)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString is null || !int.TryParse(userIdString, out var userId))
                return Unauthorized();
            var rezultat = await imagineService.SalveazaAsync(request, idEntitate, idTipImagine, userId, ct);

            if (rezultat.Success)
            {
                return CreatedAtAction(nameof(UploadImagineInfoLipsa), new { idInfoLipsa = idEntitate},
                    new ImagineResponse(rezultat.IdImagine!.Value, rezultat.NumeFisier!));
            }

            return ValidationProblem(new ValidationProblemDetails(rezultat.Errors.ToDictionary(e => e.Key, e => new[] { e.Value })));
        }

        [HttpGet("api/imagini/{id:int}")]
        [Authorize(Policy = "CanView")]
        public async Task<IActionResult> ObtineImagine(int id, CancellationToken ct)
        {
            var imagine = await imagineService.ObtineImagineAsync(id, ct);
            if (imagine == null)
                return NotFound();

            var caleCompleta = imagineService.ObtineCaleCompleta(imagine);

            if (!System.IO.File.Exists(caleCompleta))
                return NotFound();

            return PhysicalFile(caleCompleta, "image/png");
        }

        [HttpGet("api/imagini/{id:int}/thumbnail")]
        [Authorize(Policy = "CanView")]
        public async Task<IActionResult> ObtineThumbnail(int id, CancellationToken ct)
        {
            var imagine = await imagineService.ObtineThumbnailAsync(id, ct);
            if (imagine == null)
                return NotFound();

            var caleCompleta = imagineService.ObtineCaleThumbnail(imagine);

            if (!System.IO.File.Exists(caleCompleta))
                return NotFound();

            return PhysicalFile(caleCompleta, "image/png");
        }



    }
    
}
