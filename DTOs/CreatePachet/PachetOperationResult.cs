namespace PacheteAPP.DTOs.CreatePachet
{
    public sealed class PachetOperationResult
    {
        public bool Success { get; init; }
        public bool NecesitaConfirmare { get; init; }
        public int? IdPachet { get; init; }
        public string? Awb { get; init; }
        public Dictionary<string, string> Errors { get; init; } = new();

        public static PachetOperationResult Ok(int id, string awb) =>
            new() { Success = true, IdPachet = id, Awb = awb };
        
        public static PachetOperationResult Fail(string field, string message) =>
            new() { Success = false, Errors = { [field] = message } };
        public static PachetOperationResult Confirmare(string field, string message) =>
            new() { Success = false, NecesitaConfirmare = true, Errors = { [field] = message } };
    }
}
