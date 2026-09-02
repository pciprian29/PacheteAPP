using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models.Views;

namespace PacheteAPP.Pages.IstoricPages
{
    [Authorize(Roles = "Admin")]
    public class TimelineIstoricModificariModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public TimelineIstoricModificariModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<VwIstoricModificari> IstoricPachet { get; set; } = new List<VwIstoricModificari>();
        public string AwbPachet { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int id_pachet { get; set; }

        [BindProperty(SupportsGet = true)]
        public string TipObiect { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchUser { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? DataFiltru { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortData { get; set; } = "desc";

        public async Task<IActionResult> OnGetAsync(int? id_pachet)
        {
            if (id_pachet == null)
            {
                return NotFound();
            }

            this.id_pachet = id_pachet.Value;

            IQueryable<VwIstoricModificari> query = _context.VwIstoricModificari
                .AsNoTracking()
                .Where(m => m.id_pachet == this.id_pachet);

            if (!string.IsNullOrEmpty(TipObiect) && TipObiect != "Toate")
            {
                query = query.Where(m => m.descriere.Contains(TipObiect));
            }

            if (!string.IsNullOrEmpty(SearchUser))
            {
                query = query.Where(m => m.UserName.Contains(SearchUser));
            }

            if (DataFiltru.HasValue)
            {
                query = query.Where(m => m.data_modificare.HasValue &&
                                         m.data_modificare.Value.Date == DataFiltru.Value.Date);
            }

            if (SortData == "asc")
            {
                query = query.OrderBy(m => m.data_modificare);
            }
            else
            {
                query = query.OrderByDescending(m => m.data_modificare);
            }

            IstoricPachet = await query.ToListAsync();

            var primulElement = await _context.VwIstoricModificari
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.id_pachet == this.id_pachet);

            if (primulElement != null)
            {
                AwbPachet = primulElement.awb_pachet;
            }

            return Page();
        }
    }
}