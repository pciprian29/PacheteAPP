using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;
using PacheteAPP.Models.Views;

namespace PacheteAPP.Pages.DeteriorariPages;

[Authorize(Policy = "CanView")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<VwDeteriorariComplet> Deteriorare { get; set; } = default!;

    public int paginaCurenta { get; set; } = 1;
    public int paginiTotale { get; set; }
    public int pageSize = 10;
    public string searchAwbCurent { get; set; }
    public string searchLocatieCurent { get; set; }
    public string searchUtilizatorCurent { get; set; }
    public string sortCurent { get; set; }
    public int? filtruCurent { get; set; }

    public async Task OnGetAsync([FromQuery] int p = 1, [FromQuery] string? awb = null, [FromQuery] string? locatie = null,
                                 [FromQuery] string searchUtilizator = null,[FromQuery] string? sort = null, [FromQuery] int? filtru = null)
    {
        paginaCurenta = p;
        searchAwbCurent = awb;
        searchLocatieCurent = locatie;
        searchUtilizatorCurent = searchUtilizator;
        sortCurent = sort;
        filtruCurent = filtru;

        var queryBase = _context.VwDeteriorariComplet.AsQueryable();

        if (!string.IsNullOrEmpty(searchAwbCurent))
        {
            queryBase = queryBase.Where(d => d.awb.Contains(searchAwbCurent));
        }

        if (!string.IsNullOrEmpty(searchLocatieCurent))
        {
            queryBase = queryBase.Where(d => d.locatie_deteriorare.Contains(searchLocatieCurent));
        }
        if (!string.IsNullOrWhiteSpace(searchUtilizatorCurent))
        {
            queryBase = queryBase.Where(d => d.utilizator_inregistrare.Contains(searchUtilizatorCurent));
        }

        queryBase = sortCurent switch
        {
            "awb_asc" => queryBase.OrderBy(d => d.awb),
            "awb_des" => queryBase.OrderByDescending(d => d.awb),
            "utilizator_inregistrare_asc" => queryBase.OrderBy(d => d.utilizator_inregistrare),
            "utilizator_inregistrare_des" => queryBase.OrderByDescending(d => d.utilizator_inregistrare),
            "data_asc" => queryBase.OrderBy(d => d.data_inregistrare),
            "data_des" => queryBase.OrderByDescending(d => d.data_inregistrare),
            _ => queryBase.OrderBy(d => d.id_deteriorare),
        };

        int totalItems = await queryBase.CountAsync();
        paginiTotale = (int)Math.Ceiling(totalItems / (double)pageSize);

        if (paginaCurenta < 1) paginaCurenta = 1;
        if (paginaCurenta > paginiTotale && paginiTotale > 0) paginaCurenta = paginiTotale;

        var ids = await queryBase
            .Skip((paginaCurenta - 1) * pageSize)
            .Take(pageSize)
            .Select(d => d.id_deteriorare)
            .ToListAsync();

        var viewQuery = _context.VwDeteriorariComplet.Where(v => ids.Contains(v.id_deteriorare));

        viewQuery = sortCurent switch
        {
            "awb_asc" => viewQuery.OrderBy(v => v.awb),
            "awb_des" => viewQuery.OrderByDescending(v => v.awb),
            "utilizator_inregistrare_asc" => viewQuery.OrderBy(v => v.utilizator_inregistrare),
            "utilizator_inregistrare_des" => viewQuery.OrderByDescending(v => v.utilizator_inregistrare),
            "data_asc" => viewQuery.OrderBy(d => d.data_inregistrare),
            "data_des" => viewQuery.OrderByDescending(d => d.data_inregistrare),
            _ => viewQuery.OrderBy(v => v.id_deteriorare),
        };

        Deteriorare = await viewQuery.ToListAsync();
    }
}