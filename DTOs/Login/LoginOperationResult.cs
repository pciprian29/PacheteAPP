using Microsoft.CodeAnalysis.Diagnostics;

namespace PacheteAPP.DTOs.Login
{
    public class LoginOperationResult
    {
        public bool Success { get; init; }
        public string? Token { get; init; }
        public string? DisplayName { get; init; }
        public Dictionary<string, string> Errors { get; init; } = new();

        public static LoginOperationResult Ok(string token, string displayName) =>
            new() { Success = true, Token = token, DisplayName = displayName };
        public static LoginOperationResult Fail(string field, string message) =>
            new() { Success = false, Errors = { [field] = message } };
    }
}
