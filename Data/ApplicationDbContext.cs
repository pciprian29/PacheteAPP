using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Models;
using PacheteAPP.Models.Types;
using PacheteAPP.Models.Views;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Internal;

namespace PacheteAPP.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : IdentityDbContext<AppUser, IdentityRole<int>, int>(options)
    {
        
        // Tabele Principale
        public DbSet<Pachet> Pachete { get; set; }
        public DbSet<Deteriorare> Deteriorari { get; set; }
        public DbSet<InformatieLipsa> InformatiiLipsa { get; set; }
        public DbSet<Inregistrare> Inregistrari { get; set; }
        public DbSet<Client> Clienti { get; set; }
        public DbSet<Ruta> Rute { get; set; }
        public DbSet<Transport> Transporturi { get; set; }
        public DbSet<PachetTransport> Pachete_Transporturi {  get; set; }

        // Tabele tip
        public DbSet<StatusPachet> StatusPachet { get; set; }
        public DbSet<TipInregistrare> TipuriInregistrare { get; set; }
        public DbSet<TipLipsa> TipuriLipsa { get; set; }
        public DbSet<TipPachet> TipuriPachete { get; set; }
        public DbSet<TipRuta> TipuriRute { get; set; }
        public DbSet<StatusTransport> StatusTransporturi { get; set; }
        public DbSet<StatusPachetTransport> StatusPacheteTransporturi { get; set;}


        // Tabele Istoric
        public DbSet<CampTabel> CampuriTabele { get; set; }
        public DbSet<IstoricModificari> IstoricModificari { get; set; }


        // View-uri 
        public DbSet<VwIstoricModificari> VwIstoricModificari{ get; set; }
        public DbSet<VwDeteriorariComplet> VwDeteriorariComplet { get; set; }
        public DbSet<VwInfoLipsaComplet> VwInfoLipsaComplet { get; set; }
        public DbSet<VwPacheteComplet> VwPacheteComplet { get; set; }
        public DbSet<VwTransporturiComplet> VwTransporturiComplet { get; set; }
        public DbSet<VwUseriRoluri> VwUseriRoluri { get; set; }

        private int? GetCurrentUserId()
        {
            var user = httpContextAccessor.HttpContext?.User;
            if(user != null)
            {
                var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if(int.TryParse(userIdString, out int userId))
                {
                    return userId;
                }
            }
            return null;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var tabeleUrmarite = new[]
            {
                typeof(Pachet),
                typeof(Deteriorare),
                typeof(InformatieLipsa),
                typeof(Inregistrare)
            };

            var entitatiModificate = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified && tabeleUrmarite.Contains(e.Entity.GetType()))
                .ToList();


            if (entitatiModificate.Any())
            {
                var userId = GetCurrentUserId();
                var istoricNou = new List<IstoricModificari>();
                var campuriTabele = CampuriTabele.ToList();

                foreach (var entry in entitatiModificate)
                {
                    var numeTabelSql = entry.Metadata.GetTableName();
                    int idPachetAsociat = 0;

                    if (entry.Entity is Pachet p)
                    {
                        idPachetAsociat = p.id_pachet;
                    }
                    else if (entry.Entity is Inregistrare i)
                    {
                        idPachetAsociat = i.id_pachet;
                    }
                    else if (entry.Entity is Deteriorare d)
                    {
                        var inregistrareAsociata = Inregistrari.FirstOrDefault(x => x.id_inregistrare == d.id_inregistrare);
                        if (inregistrareAsociata != null) idPachetAsociat = inregistrareAsociata.id_pachet;
                    }
                    else if (entry.Entity is InformatieLipsa inf)
                    {
                        var inregistrareAsociata = Inregistrari.FirstOrDefault(x => x.id_inregistrare == inf.id_inregistrare);
                        if (inregistrareAsociata != null) idPachetAsociat = inregistrareAsociata.id_pachet;
                    }

                    if (idPachetAsociat == 0) continue;

                    foreach (var prop in entry.OriginalValues.Properties)
                    {
                        var valoareVeche = entry.OriginalValues[prop]?.ToString();
                        var valoareNoua = entry.CurrentValues[prop]?.ToString();

                        if(prop.Name == "awb" && valoareVeche == "AWB_TEMPORAR") // cred ca e o soltie buna
                        {
                            continue;
                        }

                        if (valoareNoua != valoareVeche)
                        {
                            var camp = campuriTabele.FirstOrDefault(c => c.nume_tabel == numeTabelSql && c.nume_coloana == prop.Name);
                            if (camp != null)
                            {
                                istoricNou.Add(new IstoricModificari
                                {
                                    id_pachet = idPachetAsociat,
                                    id_coloana = camp.id_coloana,
                                    valoare_veche = valoareVeche,
                                    valoare_noua = valoareNoua,
                                    data_modificare = DateTime.Now,
                                    user_modificare = userId ?? 0,
                                    descriere = $"Modificat in {numeTabelSql}: {prop.Name} din '{valoareVeche}' in '{valoareNoua}'"
                                });
                            }
                        }
                    }
                }

                if (istoricNou.Any())
                {
                    IstoricModificari.AddRange(istoricNou);
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Inregistrare>()
            .ToTable(tb =>
            {
                tb.HasTrigger("TRG_Inregistrari_DupaInsert");
                tb.HasTrigger("TRG_Inregistrari_DupaDelete");
            });

            builder.Entity<VwDeteriorariComplet>().ToView("VW_Deteriorari_Complete");
            builder.Entity<VwInfoLipsaComplet>().ToView("VW_InformatiiLipsa_Complete");
            builder.Entity<VwPacheteComplet>().ToView("VW_Pachete_Complete");
            builder.Entity<VwIstoricModificari>().ToView("VW_Istoric_Complet");
            builder.Entity <VwTransporturiComplet>().ToView("VW_Transporturi_Complet");
            builder.Entity<VwUseriRoluri>().ToView("VW_Useri_Roluri");

            //builder.Entity<Pachet>()
            //    .Property(p => p.tip_pachet)
            //    .HasConversion<string>();

            //builder.Entity<Pachet>()
            //    .Property(p => p.status_pachet)
            //    .HasConversion<string>();

            //builder.Entity<InformatieLipsa>()
            //    .Property(i => i.tip_lipsa)
            //    .HasConversion<string>();
            //builder.Entity<Inregistrare>()
            //    .Property(i => i.tip_inregistrare)
            //    .HasConversion<string>();

            // -- Tabele principale
            builder.Entity<Pachet>().ToTable("Pachete", t => t.ExcludeFromMigrations());
            builder.Entity<Inregistrare>().ToTable("Inregistrari", t => t.ExcludeFromMigrations());
            builder.Entity<Deteriorare>().ToTable("Deteriorari", t => t.ExcludeFromMigrations());
            builder.Entity<InformatieLipsa>().ToTable("InformatiiLipsa", t => t.ExcludeFromMigrations());
            builder.Entity<Client>().ToTable("Clienti", t => t.ExcludeFromMigrations());
            builder.Entity<Ruta>().ToTable("Rute", t => t.ExcludeFromMigrations());
            builder.Entity<Transport>().ToTable("Transporturi", t => t.ExcludeFromMigrations());
            builder.Entity<PachetTransport>().ToTable("Pachete_Transporturi", t => t.ExcludeFromMigrations());
            // -- Tabele tip
            builder.Entity<StatusPachet>().ToTable("StatusPachet", t => t.ExcludeFromMigrations());
            builder.Entity<TipPachet>().ToTable("TipPachet", t => t.ExcludeFromMigrations());
            builder.Entity<TipLipsa>().ToTable("TipLipsa", t => t.ExcludeFromMigrations());
            builder.Entity<TipInregistrare>().ToTable("TipInregistrare", t => t.ExcludeFromMigrations());
            builder.Entity<TipRuta>().ToTable("TipRuta", t => t.ExcludeFromMigrations());
            builder.Entity<StatusTransport>().ToTable("StatusTransport", t => t.ExcludeFromMigrations());
            builder.Entity<StatusPachetTransport>().ToTable("StatusPachetTransport", t => t.ExcludeFromMigrations());
            // -- Tabele Istoric
            builder.Entity<IstoricModificari>().ToTable("IstoricModificari", t => t.ExcludeFromMigrations());
            builder.Entity<CampTabel>().ToTable("CampuriTabele", t => t.ExcludeFromMigrations());

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