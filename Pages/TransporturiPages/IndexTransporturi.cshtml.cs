using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models.Views;

namespace PacheteAPP.Pages.TransporturiPages
{
    [Authorize(Policy = "CanView")]
    public class IndexTransporturiModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexTransporturiModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<VwTransporturiComplet> Transport { get; set; } = default!;

        public int paginaCurenta { get; set; } = 1;
        public int paginiTotale { get; set; }
        public int pageSize = 15;

        public string searchMasinaCurent { get; set; }
        public string searchSoferCurent { get; set; }
        public string sortCurent { get; set; }
        public int? statusCurent { get; set; }

        public async Task OnGetAsync([FromQuery] int p = 1, [FromQuery] string searchMasina = null, [FromQuery] string searchSofer = null,
                                    [FromQuery] string sort = null, [FromQuery] int? status = null)
        {
            paginaCurenta = p;
            searchMasinaCurent = searchMasina;
            searchSoferCurent = searchSofer;
            sortCurent = sort;
            statusCurent = status;

            var queryBase = _context.VwTransporturiComplet.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchMasinaCurent))
            {
                queryBase = queryBase.Where(x => x.numar_auto.Contains(searchMasinaCurent));
            }
            if (!string.IsNullOrWhiteSpace(searchSoferCurent))
            {
                queryBase = queryBase.Where(x => x.UserName.Contains(searchSoferCurent));
            }
            if (statusCurent.HasValue)
            {
                queryBase = queryBase.Where(x => x.id_status_transport == statusCurent.Value);
            }

            queryBase = sortCurent switch
            {
                "masina_asc" => queryBase.OrderBy(x => x.numar_auto),
                "masina_des" => queryBase.OrderByDescending(x => x.numar_auto),
                "data_asc" => queryBase.OrderBy(x => x.data_plecare),
                "data_des" => queryBase.OrderByDescending(x => x.data_plecare),
                _ => queryBase.OrderByDescending(x => x.id_transport),
            };

            int totalItems = await queryBase.CountAsync();
            paginiTotale = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (paginaCurenta < 1) paginaCurenta = 1;
            if (paginaCurenta > paginiTotale && paginiTotale > 0) paginaCurenta = paginiTotale;

            Transport = await queryBase
                .Skip((paginaCurenta - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // --- HANDLER AJAX PENTRU PACHETE ---
        public async Task<JsonResult> OnGetPacheteAsync(int idTransport)
        {
            var pachete = await _context.Pachete_Transporturi
                .Include(pt => pt.Pachet)
                .Where(pt => pt.id_transport == idTransport)
                .AsNoTracking()
                .Select(pt => new
                {
                    awb = pt.Pachet.awb,
                    adresa = pt.Pachet.adresa_destinatar, 
                    status = "In tranzit"
                })
                .ToListAsync();

            return new JsonResult(pachete);
        }
    }
}

