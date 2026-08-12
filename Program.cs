using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddQuickGridEntityFrameworkAdapter();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();


var app = builder.Build();


// ------- Teste Inserare Date in BD ---------
//using (var scope = app.Services.CreateScope()) {
//    var db = scope.ServiceProvider.GetRequiredService<PacheteAPP.Data.ApplicationDbContext>();
//    try
//    {
//        //var pachetTest = new PacheteAPP.Models.Pachet
//        //{
//        //    awb = "AWB-TEST-001",
//        //    descriere = "Testare conexiune backend",
//        //    tip_pachet = PacheteAPP.Models.TipPachet.Cutie,
//        //    nume_expeditor = "Ion Popescu",
//        //    adresa_expeditor = "Str. Lalelelor 1, Bucuresti",
//        //    nume_destinatar = "Ciprian Petrescu",
//        //    adresa_destinatar = "Str. Trandafirilor 2, Cluj",
//        //    greutate_teoretica = 2.5m,
//        //    greutate_efectiva = 2.5m,
//        //    status_pachet = PacheteAPP.Models.StatusPachet.Inregistrat,
//        //    id_user = "a84a2090-7092-490a-b9a4-32c64c40a33c"
//        //};
//        //db.Pachete.Add(pachetTest);

//        //var deteriorareTest = new PacheteAPP.Models.Deteriorare
//        //{
//        //    locatie_deteriorare = "Depozit Bucuresti",
//        //    descriere_deteriorare = "Colet gaurit in partea superioara",
//        //    pachet_deteriorat = 11
//        //};

//        //db.Deteriorari.Add(deteriorareTest);

//        var infoLipsaTest = new PacheteAPP.Models.InformatieLipsa
//        {
//            camp_afectat = "adresa_destinatar",
//            tip_lipsa = PacheteAPP.Models.TipLipsa.Lipsa,
//            descriere = "Adresa destinatarului lipseste din sistem",
//            pachet_afectat = 
//        };

//        db.InformatiiLipsa.Add(infoLipsaTest);

//        db.SaveChanges();

//        Console.WriteLine("Deteriorarea a fost adaugata cu succes");
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"Eroare la adaugarea deteriorarii: {ex.Message}");
//    }
//}





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMigrationsEndPoint();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
