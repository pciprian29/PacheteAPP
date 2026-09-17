using PacheteAPP.DTOs.CreatePachet;
using PacheteAPP.DTOs.PachetDetaliiComplete;

namespace PacheteAPP.Services.CreatePachet
{
    public interface IPachetService
    {
        Task<PachetOperationResult> CreateAsync(CreatePachetRequest request, int UserId, CancellationToken ct);
        Task<PachetDetaliiCompleteResponse?> ObtineDetaliiCompleteAsync(string awb, CancellationToken ct);
    }
}
