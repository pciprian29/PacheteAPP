using PacheteAPP.DTOs.CreateDeteriorare;

namespace PacheteAPP.Services.CreateDeteriorare
{
    public interface IDeteriorareService
    {
        Task<DeteriorareOperationResult> CreateAsync(CreateDeteriorareRequest request, int UserId, CancellationToken ct);
    }
}
