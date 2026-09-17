namespace PacheteAPP.DTOs.CreateDeteriorare
{
    public sealed record CreateDeteriorareRequest(
        string awb,
        string locatieDeteriorare,
        string descriere
    );
}
