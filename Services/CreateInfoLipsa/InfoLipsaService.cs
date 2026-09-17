using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.DTOs.CreateInfoLipsa;
using PacheteAPP.Models;
using PacheteAPP.DTOs;
using PacheteAPP.Services.CreateInfoLipsa;

namespace PacheteAPP.Services
{
    public sealed class InfoLipsaService : IInfoLipsaService
    {
        private readonly ApplicationDbContext _context;

        public InfoLipsaService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<InfoLipsaOperationResult> CreateAsync(CreateInfoLipsaRequest request, int UserId, CancellationToken ct)
        {
            var informatieLipsa = new InformatieLipsa
            {
                camp_afectat = request.campAfectat,
                descriere = request.descriereInfoLipsa,
                id_tip_lipsa = request.idtipLipsa
            };

            if (request.esteAwbLipsa)
            {
                informatieLipsa.camp_afectat = "AWB";
                _context.InformatiiLipsa.Add(informatieLipsa);
                await _context.SaveChangesAsync(ct);

                return InfoLipsaOperationResult.Ok(informatieLipsa.id_info_lipsa);
            }

            if (string.IsNullOrWhiteSpace(request.awb))
            {
                return InfoLipsaOperationResult.Fail(nameof(request.awb), "AWB-ul este obligatoriu daca nu este bifat 'AWB Lipsa'");
            }
            var pachet = await _context.Pachete
                .FirstOrDefaultAsync(p => p.awb == request.awb);

            if (pachet == null)
            {
                return InfoLipsaOperationResult.Fail(nameof(request.awb), "AWB-ul nu exista în baza de date.");
            }

            string? numeStatusCurent = await _context.StatusPachet
                .Where(s => s.id_status_pachet == pachet.id_status_pachet)
                .Select(s => s.denumire)
                .FirstOrDefaultAsync();

            int noulStatusId = pachet.id_status_pachet;

            if (numeStatusCurent == "Deteriorat")
            {
                noulStatusId = 6;
            }
            else if (numeStatusCurent != "InformatiiLipsa" && numeStatusCurent != "Deteriorat+InfoLipsa")
            {
                noulStatusId = 5;
            }

            if (pachet.id_status_pachet != noulStatusId)
            {
                pachet.id_status_pachet = noulStatusId;
            }

            var inregistrareNoua = new Inregistrare
            {
                id_pachet = pachet.id_pachet,
                id_tip_inregistrare = 2,
                id_user = UserId 
            };

            _context.Inregistrari.Add(inregistrareNoua);
            await _context.SaveChangesAsync(ct);

            informatieLipsa.id_inregistrare = inregistrareNoua.id_inregistrare;
            _context.InformatiiLipsa.Add(informatieLipsa);
            await _context.SaveChangesAsync(ct);

            return InfoLipsaOperationResult.Ok(informatieLipsa.id_info_lipsa);
        }
    }
}