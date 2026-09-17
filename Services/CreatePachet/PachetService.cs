using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.DTOs.CreatePachet;
using PacheteAPP.DTOs.PachetDetaliiComplete;
using PacheteAPP.Models;
using PacheteAPP.Models.Helper;
using System.Text.RegularExpressions;

namespace PacheteAPP.Services.CreatePachet
{
    public sealed class PachetService(ApplicationDbContext context): IPachetService
    {
        private static readonly Regex EmailRegex = new (@"(?<![a-zA-Z@.-])[a-zA-Z0-9]+@[a-zA-Z]+\.[a-z]+(?![A-Za-z@.-])");

        public async Task<PachetOperationResult> CreateAsync(
            CreatePachetRequest request, int UserId, CancellationToken ct)
        {
            if (!EmailRegex.IsMatch(request.emailExpeditor))
            {
                return PachetOperationResult.Fail(nameof(request.emailExpeditor), "Email expeditor invalid");
            }
            if(!EmailRegex.IsMatch(request.emailDestinatar))
            {
                return PachetOperationResult.Fail(nameof(request.emailDestinatar), "Email destinatar invalid");
            }
            if(request.tipExpeditor == "nou" && (string.IsNullOrEmpty(request.numeExpeditor) || string.IsNullOrEmpty(request.prenumeExpeditor)))
            {
                return PachetOperationResult.Fail("numeExpeditor", "Nume si prenume expeditor incomplete");
            }
            if(request.tipDestinatar == "nou" && (string.IsNullOrEmpty(request.numeDestinatar) || string.IsNullOrEmpty(request.prenumeDestinatar)))
            {
                return PachetOperationResult.Fail("numeDestinatar", "Nume si prenume destinatar incomplete");
            }
            if(request.greutateTeoretica <= 0)
            {
                return PachetOperationResult.Fail("greutateTeoretica", "Greutate Teoretica Invalida");
            }
            if (request.greutateEfectiva <= 0)
            {
                return PachetOperationResult.Fail("greutateEfectiva", "Greutate Efectiva Invalida");
            }
            if(request.idTipPachet <= 0)
            {
                return PachetOperationResult.Fail("IdTipPachet", "Tip pachet invalid");
            }
            if(request.idStatusPachet <= 0)
            {
                return PachetOperationResult.Fail("idStatusPachet", "Status pachet invalid");
            }

            // Expeditor

            if (request.tipExpeditor == "existent")
            {
                var existent = await context.Clienti.FindAsync([request.emailExpeditor], ct);
                if(existent is null)
                {
                    return PachetOperationResult.Fail("emailExpeditor", "Acest mail nu corespunde unui client existent");
                }
                else if(request.numeExpeditor != null || request.prenumeExpeditor != null)
                {
                    return PachetOperationResult.Fail("numeExpeditor", "Numele expeditorului nu poate fi adaugat pentru un client existent");
                }
            }else if(request.tipExpeditor == "nou")
            {
                var existent = await context.Clienti.FindAsync([request.emailExpeditor], ct);
                if(existent is null)
                {
                    context.Clienti.Add(new Client
                    {
                        email_client = request.emailExpeditor,
                        nume = request.numeExpeditor,
                        prenume = request.prenumeExpeditor,
                        expeditor_destinatar = false
                    });
                }
                else if (request.confirmaSuprascriereExpeditor != true) 
                {
                    return PachetOperationResult.Confirmare("emailExpeditor", "Acest email exista deja, confirma suprascrierea");
                }
                else
                {
                    existent.nume = request.numeExpeditor;
                    existent.prenume = request.prenumeExpeditor;
                }
            }

            //Destinatar

            if (request.tipDestinatar == "existent")
            {
                var existent = await context.Clienti.FindAsync([request.emailDestinatar], ct);
                if (existent is null)
                {
                    return PachetOperationResult.Fail("emailDestinatar", "Acest mail nu corespunde unui client existent");
                }
                else if (request.numeDestinatar != null || request.prenumeDestinatar != null)
                {
                    return PachetOperationResult.Fail("numeExpeditor", "Numele destinatarului nu poate fi adaugat pentru un client existent");
                }
            }
            else if (request.tipDestinatar == "nou")
            {
                var existent = await context.Clienti.FindAsync([request.emailDestinatar], ct);
                if (existent is null)
                {
                    context.Clienti.Add(new Client
                    {
                        email_client = request.emailDestinatar,
                        nume = request.numeDestinatar,
                        prenume = request.prenumeDestinatar,
                        expeditor_destinatar = true
                    });
                }
                else if (request.confirmaSuprascriereDestinatar != true)
                {
                    return PachetOperationResult.Confirmare("emailDestinatar", "Acest email exista deja, confirma suprascrierea");
                }
                else
                {
                    existent.nume = request.numeDestinatar;
                    existent.prenume = request.prenumeDestinatar;
                }
            }

            await context.SaveChangesAsync(ct);

            var pachet = new Pachet
            {
                email_expeditor = request.emailExpeditor,
                email_destinatar = request.emailDestinatar,
                id_tip_pachet = request.idTipPachet,
                descriere = request.descriere,
                greutate_teoretica = request.greutateTeoretica,
                greutate_efectiva = request.greutateEfectiva,
                adresa_expeditor = request.adresaExpeditor,
                adresa_destinatar = request.adresaDestinatar,
                id_status_pachet = request.idStatusPachet,
                awb = "AWB_TEMPORAR"
            };

            context.Pachete.Add(pachet);
            await context.SaveChangesAsync(ct);

            pachet.awb = GeneratorAwb.GenerareAwb(pachet.id_pachet);
            context.Pachete.Update(pachet);
            await context.SaveChangesAsync(ct);

            context.Inregistrari.Add(new Inregistrare
            {
                id_pachet = pachet.id_pachet,
                id_tip_inregistrare = 1,
                id_user = UserId
            });
            await context.SaveChangesAsync(ct);

            return PachetOperationResult.Ok(pachet.id_pachet, pachet.awb);

        }

        public async Task<PachetDetaliiCompleteResponse> ObtineDetaliiCompleteAsync(string awb, CancellationToken ct)
        {
            var pachet = await context.Pachete
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.awb == awb, ct);

            if (pachet == null) return null;

            var idInregistrariPachet = context.Inregistrari
                .Where(i => i.id_pachet == pachet.id_pachet)
                .Select(i => i.id_inregistrare);

            var deteriorari = await context.Deteriorari
                .Where(d => idInregistrariPachet.Contains(d.id_inregistrare))
                .ToListAsync(ct);

            var infoLipsa = await context.InformatiiLipsa
                .Where(inf => inf.id_inregistrare != null && idInregistrariPachet.Contains(inf.id_inregistrare.Value))
                .ToListAsync(ct);

            var idsDeteriorari = deteriorari.Select(d => d.id_deteriorare).ToList();
            var idsInfoLipsa = infoLipsa.Select(i => i.id_info_lipsa).ToList();

            var imaginiDeteriorare = await context.Imagini
                .Where(im => im.id_tip_imagine == 2 && idsDeteriorari.Contains(im.id_entitate))
                .ToListAsync(ct);
            var imaginiInfoLipsa = await context.Imagini
                .Where(im => im.id_tip_imagine == 1 && idsInfoLipsa.Contains(im.id_entitate))
                .ToListAsync(ct);

            ImagineDetaliiDto CatreDto(Imagine im) =>
                new(im.id_imagine, $"/api/imagini/{im.id_imagine}", im.data_creare);

            var pachetDto = new PachetDetaliiDto(
                pachet.id_pachet,
                pachet.awb,
                pachet.descriere,
                pachet.adresa_expeditor,
                pachet.adresa_destinatar,
                pachet.greutate_teoretica,
                pachet.greutate_efectiva,
                pachet.id_tip_pachet,
                pachet.id_status_pachet,
                pachet.email_expeditor,
                pachet.email_destinatar,
                pachet.numar_deteriorari,
                pachet.numar_infolipsa
                );
            var infoLipsaDto = infoLipsa.Select(i => new InfoLipsaDetaliiDto(
                i.id_info_lipsa,
                i.camp_afectat,
                i.descriere,
                i.id_tip_lipsa,
                imaginiInfoLipsa.Where(im => im.id_entitate == i.id_info_lipsa).Select(CatreDto).ToList()
            )).ToList();

            var deteriorariDto = deteriorari.Select(d => new DeteriorareDetaliiDto(
                d.id_deteriorare,
                d.locatie_deteriorare,
                d.descriere_deteriorare,
                imaginiDeteriorare.Where(im => im.id_entitate == d.id_deteriorare).Select(CatreDto).ToList()
            )).ToList();

            return new PachetDetaliiCompleteResponse(
                pachetDto,
                infoLipsaDto,
                deteriorariDto
            );
        }
    }
}
