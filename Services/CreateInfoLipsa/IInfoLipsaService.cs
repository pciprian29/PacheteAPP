using PacheteAPP.DTOs.CreateInfoLipsa;

namespace PacheteAPP.Services.CreateInfoLipsa
{
    public interface IInfoLipsaService
    {
        public Task<InfoLipsaOperationResult> CreateAsync(CreateInfoLipsaRequest request, int UserId, CancellationToken ct);
    }
}
