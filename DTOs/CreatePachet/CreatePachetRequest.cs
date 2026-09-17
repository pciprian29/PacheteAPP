namespace PacheteAPP.DTOs.CreatePachet;
public sealed record CreatePachetRequest(
    int idTipPachet,
    string descriere,
    decimal greutateTeoretica,
    decimal greutateEfectiva,

    string tipExpeditor,
    string emailExpeditor,
    string adresaExpeditor,
    string? numeExpeditor,
    string? prenumeExpeditor,
    bool? confirmaSuprascriereExpeditor,

    string tipDestinatar,
    string emailDestinatar,
    string adresaDestinatar,
    string? numeDestinatar,
    string? prenumeDestinatar,
    bool? confirmaSuprascriereDestinatar,

    int idStatusPachet
    );

