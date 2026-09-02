using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Models;
using PacheteAPP.Models.Views;
using PacheteAPP.Models.Types;
using PacheteAPP.Data;
using Microsoft.AspNetCore.Authorization;

namespace PacheteAPP.Pages.PachetPages
{
    [Authorize(Policy = "CanView")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<VwPacheteComplet> Pachet { get; set; } = default!;

        public int paginaCurenta { get; set; } = 1;
        public int paginiTotale { get; set; }
        public int pageSize = 10;
        public string searchAwbCurent { get; set; }
        public string searchExpeditorCurent { get; set; }
        public string searchDestinatarCurent { get; set; }

        public string sortCurent { get; set; }
        public int? filtruCurent { get; set; }
        public int? statusCurent { get; set; }

        public async Task OnGetAsync([FromQuery] int p = 1, [FromQuery] string searchAwb = null, [FromQuery] string searchExpeditor = null, [FromQuery] string searchDestinatar = null,
                                    [FromQuery] string sort = null, [FromQuery] int? filtru = null, [FromQuery] int? status = null)
        {
            paginaCurenta = p;
            searchAwbCurent = searchAwb;
            searchExpeditorCurent = searchExpeditor;
            searchDestinatarCurent = searchDestinatar;
            sortCurent = sort;
            filtruCurent = filtru;
            statusCurent = status;

            var queryBase = _context.Pachete.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchAwbCurent))
            {
                queryBase = queryBase.Where(x => x.awb.Contains(searchAwbCurent));
            }

            if (!string.IsNullOrWhiteSpace(searchExpeditorCurent))
            {
                queryBase = queryBase.Where(x => x.email_expeditor.Contains(searchExpeditorCurent));
            }
            if (!string.IsNullOrWhiteSpace(searchDestinatarCurent))
            {
                queryBase = queryBase.Where(x => x.email_destinatar.Contains(searchDestinatarCurent));
            }

            if (filtruCurent.HasValue)
            {
                queryBase = queryBase.Where(x => x.id_tip_pachet == filtruCurent.Value);
            }
            if (statusCurent.HasValue)
            {
                queryBase = queryBase.Where(x => x.id_status_pachet == statusCurent.Value);
            }

            queryBase = sortCurent switch
            {
                "awb_asc" => queryBase.OrderBy(x => x.awb),
                "awb_des" => queryBase.OrderByDescending(x => x.awb),
                "email_expeditor_asc" => queryBase.OrderBy(x => x.email_expeditor),
                "email_expeditor_des" => queryBase.OrderByDescending(x => x.email_expeditor),
                "email_destinatar_asc" => queryBase.OrderBy(x => x.email_destinatar),
                "email_destinatar_des" => queryBase.OrderByDescending(x => x.email_destinatar),
                "nr_deteriorari_asc" => queryBase.OrderBy(x => x.numar_deteriorari),
                "nr_deteriorari_des" => queryBase.OrderByDescending(x => x.numar_deteriorari),
                "nr_infolipsa_asc" => queryBase.OrderBy(x => x.numar_infolipsa),
                "nr_infolipsa_des" => queryBase.OrderByDescending(x => x.numar_infolipsa),
                _ => queryBase.OrderBy(x => x.id_pachet),
            };

            int totalItems = await queryBase.CountAsync();

            paginiTotale = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (paginaCurenta < 1) paginaCurenta = 1;
            if (paginaCurenta > paginiTotale && paginiTotale > 0) paginaCurenta = paginiTotale;

            var ids = await queryBase
                .Skip((paginaCurenta - 1) * pageSize)
                .Take(pageSize)
                .Select(p => p.id_pachet)
                .ToListAsync();

            var viewQuery = _context.VwPacheteComplet.Where(v => ids.Contains(v.id_pachet));

            viewQuery = sortCurent switch
            {
                "awb_asc" => viewQuery.OrderBy(v => v.awb),
                "awb_des" => viewQuery.OrderByDescending(v => v.awb),
                "email_expeditor_asc" => viewQuery.OrderBy(v => v.email_expeditor),
                "email_expeditor_des" => viewQuery.OrderByDescending(v => v.email_expeditor),
                "email_destinatar_asc" => viewQuery.OrderBy(v => v.email_destinatar),
                "email_destinatar_des" => viewQuery.OrderByDescending(v => v.email_destinatar),
                "nr_deteriorari_asc" => viewQuery.OrderBy(v => v.numar_deteriorari),
                "nr_deteriorari_des" => viewQuery.OrderByDescending(x => x.numar_deteriorari),
                "nr_infolipsa_asc" => viewQuery.OrderBy(x => x.numar_infolipsa),
                "nr_infolipsa_des" => viewQuery.OrderByDescending(x => x.numar_infolipsa),
                _ => viewQuery.OrderBy(v => v.id_pachet),
            };
                
            Pachet = await viewQuery.ToListAsync();
        }
    }
}