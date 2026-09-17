namespace PacheteAPP.DTOs.Imagini
{
    public class ImagineOperationResult
    {
        public bool Success { get; init; }
        public int? IdImagine { get; init; }
        public string? NumeFisier { get; init; }
        public Dictionary<string, string> Errors { get; init; } = new();


        public static ImagineOperationResult Ok(int id, string numeFisier) =>
            new() { Success = true, IdImagine = id, NumeFisier = numeFisier };

        public static ImagineOperationResult Fail(string field, string message) =>
            new() { Success = false, Errors = { [field] = message } };
    }
}
