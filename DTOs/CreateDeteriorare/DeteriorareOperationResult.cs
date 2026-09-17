namespace PacheteAPP.DTOs.CreateDeteriorare
{
    public sealed class DeteriorareOperationResult
    {
        public bool Success { get; init; }
        public int IdDeteriorare { get; init; }
        public string? Awb { get; init; }
        public Dictionary<string, string> Errors { get; init; } = new();

        public static DeteriorareOperationResult Ok(int id, string awb) =>
            new() { Success = true, IdDeteriorare = id, Awb = awb };

        public static DeteriorareOperationResult Fail(string field, string message) =>
            new() { Success = false, Errors = { [field] = message } };
    }
}
