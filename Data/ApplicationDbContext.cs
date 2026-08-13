using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Models;

namespace PacheteAPP.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<AppUser, IdentityRole<int>, int>(options)
    {
        public DbSet<Pachet> Pachete { get; set; }
        public DbSet<Deteriorare> Deteriorari { get; set; }
        public DbSet<InformatieLipsa> InformatiiLipsa { get; set; }
        public DbSet<Inregistrare> Inregistrari { get; set; }
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
            builder.Entity<Inregistrare>()
                .Property(i => i.tip_inregistrare)
                .HasConversion<string>();

            builder.Entity<Pachet>().ToTable("pachete", t => t.ExcludeFromMigrations());
            builder.Entity<Inregistrare>().ToTable("inregistrari", t => t.ExcludeFromMigrations());
            builder.Entity<Deteriorare>().ToTable("deteriorari", t => t.ExcludeFromMigrations());
            builder.Entity<InformatieLipsa>().ToTable("informatii_lipsa", t => t.ExcludeFromMigrations());
            builder.Entity<IstoricModificari>().ToTable("istoric_modificari", t => t.ExcludeFromMigrations());
            builder.Entity<CampTabel>().ToTable("campuri_tabele", t => t.ExcludeFromMigrations());

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(string))
                    {
                        property.SetIsUnicode(false);
                    }
                }
            }
        }
    }
}