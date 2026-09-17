namespace PacheteAPP.DTOs.CreateInfoLipsa
{
    public sealed record CreateInfoLipsaRequest(
        bool esteAwbLipsa,
        string? awb,
        string campAfectat,
        string descriereInfoLipsa,
        int  idtipLipsa
        );
}
