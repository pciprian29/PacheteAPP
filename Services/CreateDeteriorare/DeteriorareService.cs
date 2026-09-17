using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.DTOs.CreateDeteriorare;

namespace PacheteAPP.Services.CreateDeteriorare
{
    public sealed class DeteriorareService(ApplicationDbContext context): IDeteriorareService
    {
        public async Task<DeteriorareOperationResult> CreateAsync(
            CreateDeteriorareRequest request, int UserId, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(request.awb))
            {
                return DeteriorareOperationResult.Fail(nameof(request.awb), "Awb-ul este obligatoriu");
            }
            var PachetId = await context.Pachete
                .Where(p => p.awb == request.awb)
                .Select(p => p.id_pachet)
                .FirstOrDefaultAsync();
            if(PachetId == 0)
            {
                return DeteriorareOperationResult.Fail(nameof(request.awb), "Nu exista un pachet cu acest awb");
            }
            if (string.IsNullOrEmpty(request.locatieDeteriorare))
            {
                return DeteriorareOperationResult.Fail(nameof(request.locatieDeteriorare), "Locatia Deteriorarii este obligatorie");
            }
            if (string.IsNullOrEmpty(request.descriere))
            {
                return DeteriorareOperationResult.Fail(nameof(request.descriere), "Descrierea este obligatorie");
            }

            var pachet = await context.Pachete.FindAsync(PachetId);

            if (pachet != null)
            {
                string? numeStatusCurent = await context.StatusPachet
                    .Where(s => s.id_status_pachet == pachet.id_status_pachet)
                    .Select(s => s.denumire)
                    .FirstOrDefaultAsync();

                int noulStatusId = pachet.id_status_pachet;

                if (numeStatusCurent == "InformatiiLipsa")
                {
                    noulStatusId = 6;
                }
                else if (numeStatusCurent != "Deteriorat" && numeStatusCurent != "Deteriorat+InfoLipsa")
                {
                    noulStatusId = 4;
                }

                if (pachet.id_status_pachet != noulStatusId)
                {
                    pachet.id_status_pachet = noulStatusId;
                }

                var inregistrareNoua = new Inregistrare
                {
                    id_pachet = PachetId,
                    id_tip_inregistrare = 3,
                    id_user = UserId,
                };

                context.Inregistrari.Add(inregistrareNoua);

                await context.SaveChangesAsync(ct);

                var Deteriorare = new Deteriorare
                {
                    locatie_deteriorare = request.locatieDeteriorare,
                    descriere_deteriorare = request.descriere,
                };

                Deteriorare.id_inregistrare = inregistrareNoua.id_inregistrare;
                context.Deteriorari.Add(Deteriorare);

                await context.SaveChangesAsync(ct);

                return DeteriorareOperationResult.Ok(Deteriorare.id_deteriorare, request.awb);
            }

            return DeteriorareOperationResult.Fail(nameof(request.awb), "Nu exista un pachet cu acest awb");
        }
    }
}
