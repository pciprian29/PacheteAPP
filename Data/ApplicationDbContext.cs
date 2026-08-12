using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Models;

namespace PacheteAPP.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Pachet> Pachete { get; set; }
        public DbSet<Deteriorare> Deteriorari { get; set; }
        public DbSet<InformatieLipsa> InformatiiLipsa { get; set; }
        public DbSet<CampTabel> CampuriTabele { get; set; }
        public DbSet<IstoricModificari> IstoricModificari { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Pachet>()
                .Property(p => p.tip_pachet)
                .HasConversion<string>();

            builder.Entity<Pachet>()
                .Property(p => p.status_pachet)
                .HasConversion<string>();

            builder.Entity<InformatieLipsa>()
                .Property(i => i.tip_lipsa)
                .HasConversion<string>();
        }
    }
}
