using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models.Views;
using PacheteAPP.Pages.InfoLipsaPages;

namespace PacheteAPP.Pages.IstoricPages
{
    [Authorize(Roles = "Admin")]
    public class IndexIstoricModificariModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexIstoricModificariModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<VwIstoricModificari> IstoricModificari { get; set; } = new List<VwIstoricModificari>();

        public int paginaCurenta { get; set; } = 1;
        public int paginiTotale { get; set; }
        public int pageSize = 10;
        public string? awbCurent { get; set; }
        public string? sortCurent { get; set; }
        public async Task OnGetAsync([FromQuery] int p = 1 ,[FromQuery] string? awb = null, [FromQuery] string? sort = null)
        {
            paginaCurenta = p;
            awbCurent = awb;
            sortCurent = sort;

            var queryBase = _context.VwIstoricModificari.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(awbCurent))
            {
                queryBase = queryBase.Where(m => m.awb_pachet.Contains(awbCurent));
            }

            queryBase = sortCurent switch
            {
                "awb_asc" => queryBase.OrderBy(m => m.awb_pachet),
                "awb_des" => queryBase.OrderByDescending(m => m.awb_pachet),
                "data_asc" => queryBase.OrderBy(m => m.data_modificare),
                "data_des" => queryBase.OrderByDescending(m => m.data_modificare),
                _ => queryBase.OrderBy(m => m.id_modificare)
            };

            int totalItems = await queryBase.CountAsync();
            paginiTotale = (int)Math.Ceiling(totalItems / (double)pageSize);

            if(paginaCurenta < 1) paginaCurenta = 1;
            if(paginaCurenta > paginiTotale && paginiTotale > 0) paginaCurenta = paginiTotale;

            var ids = await queryBase
                .Skip((paginaCurenta - 1)* pageSize)
                .Take(pageSize)
                .Select(m => m.id_modificare)
                .ToListAsync();

            var viewQuery = _context.VwIstoricModificari.Where(v => ids.Contains(v.id_modificare));

            viewQuery = sortCurent switch
            {
                "awb_asc" => viewQuery.OrderBy(m => m.awb_pachet),
                "awb_des" => viewQuery.OrderByDescending(m => m.awb_pachet),
                _ => viewQuery.OrderBy(m => m.id_modificare)
            };

            IstoricModificari = await viewQuery.ToListAsync();
        }
    }
}
