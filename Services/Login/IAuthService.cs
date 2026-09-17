using Microsoft.Identity.Client;
using PacheteAPP.DTOs.Login;

namespace PacheteAPP.Services.Login
{
    public interface IAuthService
    {
        Task<LoginOperationResult> LoginAsync(AndroidLoginRequest request, CancellationToken ct);
    }
}
