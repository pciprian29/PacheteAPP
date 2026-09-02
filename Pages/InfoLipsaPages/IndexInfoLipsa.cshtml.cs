using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;
using PacheteAPP.Models.Views;

namespace PacheteAPP.Pages.InfoLipsaPages;

[Authorize(Policy = "CanView")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<InformatieLipsa> InformatieLipsa { get; set; } = default!;

    public IList<VwInfoLipsaComplet> InformatiiLipsa { get; set; } = default!;

    public int paginaCurenta { get; set; } = 1;

    public int paginiTotale { get; set; }

    public int pageSize = 10;
    public string searchAwbCurent { get; set;}
    public string searchCampCurent { get; set; }
    public string searchUtilizatorCurent { get; set; }
    public string sortCurent { get; set; }
    public string filtruTipCurent { get; set; }
    public async Task OnGetAsync([FromQuery] int p = 1, [FromQuery] string searchAwb = null,[FromQuery] string searchCamp = null ,
                                [FromQuery] string searchUtilizator = null,[FromQuery] string sort = null, [FromQuery] string filtru = null)
    {
        paginaCurenta = p;
        searchAwbCurent = searchAwb;
        searchCampCurent = searchCamp;
        searchUtilizatorCurent = searchUtilizator;
        sortCurent = sort;
        filtruTipCurent = filtru;

        var queryBase = _context.VwInfoLipsaComplet.AsQueryable();

        if(!string.IsNullOrEmpty(searchAwbCurent))
        {
            queryBase = queryBase.Where(x => x.awb.Contains(searchAwbCurent));
        }

        if (!string.IsNullOrWhiteSpace(searchCampCurent))
        {
            queryBase = queryBase.Where(x => x.camp_afectat.Contains(searchCampCurent));
        }

        if (!string.IsNullOrWhiteSpace(searchUtilizatorCurent))
        {
            queryBase = queryBase.Where(x => x.utilizator_inregistrare.Contains(searchUtilizatorCurent));
        }
        if (!string.IsNullOrWhiteSpace(filtruTipCurent))
        {
            queryBase = queryBase.Where(x => x.tip_lipsa == filtruTipCurent);
        }

        queryBase = sortCurent switch
        {
            "awb_asc" => queryBase.OrderBy(x => x.awb),
            "awb_des" => queryBase.OrderByDescending(x => x.awb),
            "utilizator_asc" => queryBase.OrderBy(x => x.utilizator_inregistrare),
            "utilizator_des" => queryBase.OrderByDescending(x => x.utilizator_inregistrare),
            "data_asc" => queryBase.OrderBy(v => v.data_inregistrare),
            "data_des" => queryBase.OrderByDescending(v => v.data_inregistrare),

            _ => queryBase.OrderBy(x => x.id_info_lipsa),
        };

        int totalItems = await queryBase.CountAsync();

        paginiTotale = (int)Math.Ceiling(totalItems / (double)pageSize);

        if (paginaCurenta < 1) paginaCurenta = 1;
        if (paginaCurenta > paginiTotale && paginiTotale > 0) paginaCurenta = paginiTotale;

        var ids = await queryBase
            .OrderBy(i => i.id_info_lipsa)
            .Skip((paginaCurenta - 1) * pageSize)
            .Take(pageSize)
            .Select(i => i.id_info_lipsa)
            .ToListAsync();

        var viewQuery = _context.VwInfoLipsaComplet.Where(v => ids.Contains(v.id_info_lipsa));

        viewQuery = sortCurent switch
        {
            "awb_asc" => viewQuery.OrderBy(v => v.awb),
            "awb_des" => viewQuery.OrderByDescending(v => v.awb),
            "utilizator_asc" => viewQuery.OrderBy(v => v.utilizator_inregistrare),
            "utilizator_des" => viewQuery.OrderByDescending(v => v.utilizator_inregistrare),
            "data_asc" => viewQuery.OrderBy(v => v.data_inregistrare),
            "data_des" => viewQuery.OrderByDescending(v => v.data_inregistrare),
            _ => viewQuery.OrderBy(v => v.id_info_lipsa),
        };

        InformatiiLipsa = await viewQuery.ToListAsync();
    }
}
