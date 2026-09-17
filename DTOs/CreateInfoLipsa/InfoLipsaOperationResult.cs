using System.Runtime.InteropServices;

namespace PacheteAPP.DTOs.CreateInfoLipsa
{
    public sealed class InfoLipsaOperationResult
    {
        public bool Success { get; init; }
        public int? idInfoLipsa { get; init; }

        public Dictionary<string, string> Errors { get; init; } = new();

        public static InfoLipsaOperationResult Ok(int id)=>
            new () { Success = true, idInfoLipsa = id };

        public static InfoLipsaOperationResult Fail(string field, string message) =>
            new() { Success = false, Errors = { [field] = message }};
    }
}
