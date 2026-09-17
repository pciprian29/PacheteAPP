using PacheteAPP.DTOs.Imagini;

namespace PacheteAPP.Services.Imagini
{
    public interface IImagineService
    {
        Task<ImagineOperationResult> SalveazaAsync(
            UploadImagineRequest request, int idEntitate, int idTipImagine, int idUser, CancellationToken ct);

        Task<Imagine?> ObtineImagineAsync(int idImagine, CancellationToken ct);
        Task<Imagine?> ObtineThumbnailAsync(int idImagine, CancellationToken ct);

        string ObtineCaleCompleta(Imagine imagine);
        string ObtineCaleThumbnail(Imagine imagine);
    }
}