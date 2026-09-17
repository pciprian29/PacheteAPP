namespace PacheteAPP.DTOs.PachetDetaliiComplete
{
    public sealed record ImagineDetaliiDto(int IdImagine, string Url, DateTime DataCreare);

    public sealed record PachetDetaliiDto(
        int IdPachet,
        string Awb,
        string Descriere,
        string AdresaExpeditor,
        string AdresaDestinatar,
        decimal GreutateTeoretica,
        decimal GreutateEfectiva,
        int IdTipPachet,
        int IdStatusPachet,
        string EmailExpeditor,
        string EmailDestinatar,
        int NumarDeteriorari,
        int NumarInfoLipsa
        );

    public sealed record InfoLipsaDetaliiDto(
        int IdInfoLipsa,
        string CampAfectat,
        string Descriere,
        int IdTipLipsa,
        List<ImagineDetaliiDto> Imagini
        );

    public sealed record DeteriorareDetaliiDto(
        int IdDeteriorare,
        string LocatieDeteriorare,
        string DescriereDeteriorare,
        List<ImagineDetaliiDto> Imagini
        );

    public sealed record PachetDetaliiCompleteResponse(
        PachetDetaliiDto Pachet,
        List<InfoLipsaDetaliiDto> InfoLipsa,
        List<DeteriorareDetaliiDto> Deteriorari
        );
}
