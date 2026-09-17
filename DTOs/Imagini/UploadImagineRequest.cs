namespace PacheteAPP.DTOs.Imagini
{
    public sealed class UploadImagineRequest
    {
        public required IFormFile Fisier { get; set; }
        public string? Awb { get; set; }
    }
}
