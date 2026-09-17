using PacheteAPP.DTOs.Imagini;
using PacheteAPP.Models;
using PacheteAPP.Models.Helper;
using PacheteAPP.Data;
using Microsoft.IdentityModel.Tokens.Experimental;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace PacheteAPP.Services.Imagini
{
    public sealed class ImagineService(ApplicationDbContext context, IConfiguration configuration) : IImagineService
    {
        private const long DimensiuneMaxima = 10 * 1024 * 1024;
        private const string ContentTypeAsteptat = "image/png";

        public async Task<ImagineOperationResult> SalveazaAsync(UploadImagineRequest request, int idEntitate, int idTipImagine,
            int idUser, CancellationToken ct)
        {
            var fisier = request.Fisier;

            if (fisier == null || fisier.Length == 0)
            {
                return ImagineOperationResult.Fail("Fisier", "Fisier lipseste sau este gol");
            }
            if (fisier.Length > DimensiuneMaxima)
            {
                return ImagineOperationResult.Fail("Fisier", "Fisierul depaseste dimensiunea maxima acceptata");
            }
            if (!string.Equals(fisier.ContentType, ContentTypeAsteptat, StringComparison.OrdinalIgnoreCase))
            {
                return ImagineOperationResult.Fail("Fisier", "Fisierul nu respecat formatul acceptat(PNG).");
            }

            var dataAcum = DateTime.UtcNow;

            var numeFisier = GeneratorNumeImagine.Genereaza(request.Awb, idEntitate, idTipImagine, dataAcum);

            var caleBaza = configuration["StorageImagini:CaleBaza"]
                ?? throw new InvalidOperationException("StorageImagini:CaleBaza nu este configurata");

            var folderZi = Path.Combine(
                caleBaza,
                dataAcum.Year.ToString(),
                dataAcum.Month.ToString("D2"),
                dataAcum.Day.ToString("D2"));

            Directory.CreateDirectory(folderZi);

            var caleCompleta = Path.Combine(folderZi, numeFisier);
            var caleThumbnail = Path.Combine(folderZi, "thumb_" + numeFisier);
            await using (var stream = new FileStream(caleCompleta, FileMode.CreateNew, FileAccess.Write))
            {
                await fisier.CopyToAsync(stream, ct);
            }

            try
            {
                using var thumb = await Image.LoadAsync(caleCompleta, ct);
                thumb.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(200, 200),
                    Mode = ResizeMode.Max
                }));
                await thumb.SaveAsPngAsync(caleThumbnail, ct);
            }
            catch (Exception ex)
            {
                
            }

            var imagine = new Imagine
            {
                nume_fisier = numeFisier,
                id_tip_imagine = idTipImagine,
                id_entitate = idEntitate,
                id_user = idUser,
                data_creare = dataAcum
            };  

            context.Imagini.Add(imagine);
            await context.SaveChangesAsync(ct);

            return ImagineOperationResult.Ok(imagine.id_imagine, imagine.nume_fisier);
        }


        public async Task<Imagine?> ObtineImagineAsync(int idImagine, CancellationToken ct)
        {
            return await context.Imagini.FindAsync(new object[] { idImagine }, ct);
        }
        public async Task<Imagine?> ObtineThumbnailAsync(int idImagine, CancellationToken ct)
        {
            return await context.Imagini.FindAsync(new object[] { idImagine }, ct);
        }
        public string ObtineCaleCompleta(Imagine imagine)
        {
            var caleBaza = configuration["StorageImagini:CaleBaza"]
                ?? throw new InvalidOperationException("StorageImagini:CaleBaza nu este configurata");

            return Path.Combine(
                caleBaza,
                imagine.data_creare.Year.ToString(),
                imagine.data_creare.Month.ToString("D2"),
                imagine.data_creare.Day.ToString("D2"),
                imagine.nume_fisier);
        }

        public string ObtineCaleThumbnail(Imagine imagine)
        {
            var caleBaza = configuration["StorageImagini:CaleBaza"]
                ?? throw new InvalidOperationException("StorageImagini:CaleBaza nu este configurata");
            return Path.Combine(
                caleBaza,
                imagine.data_creare.Year.ToString(),
                imagine.data_creare.Month.ToString("D2"),
                imagine.data_creare.Day.ToString("D2"),
                "thumb_" + imagine.nume_fisier);
        }
    }
}