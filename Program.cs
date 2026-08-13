using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PacheteAPP.Data;
using PacheteAPP.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddQuickGridEntityFrameworkAdapter();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// modificare db
builder.Services.AddRazorPages();

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();


//// ------- Teste Inserare Date in BD ---------
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<PacheteAPP.Data.ApplicationDbContext>();
//    try
//    {
//        Console.WriteLine("--- INCEPERE TEST FLUX DATE ---");

//        // 1. Asiguram existenta unui User (pentru a avea un ID valid de tip INT)
//        var testUser = db.Users.FirstOrDefault(u => u.UserName == "curier_test");
//        if (testUser == null)
//        {
//            testUser = new PacheteAPP.Models.AppUser
//            {
//                UserName = "curier_test",
//                Email = "curier@test.ro",
//                EmailConfirmed = true
//            };
//            db.Users.Add(testUser);
//            db.SaveChanges(); // Salvam ca sa ne genereze automat Id-ul in SQL
//            Console.WriteLine($"> User creat cu ID-ul: {testUser.Id}");
//        }

//        // Generam un AWB unic aleatoriu pentru a nu pica testul la rulari multiple (ai pus constrangere UNIQUE pe awb)
//        string awbTest = "AWB-" + DateTime.Now.ToString("HHmmss");

//        // 2. Cream Pachetul independent
//        var pachetTest = new PacheteAPP.Models.Pachet
//        {
//            awb = awbTest,
//            descriere = "Testare flux complet arhitectura noua",
//            tip_pachet = PacheteAPP.Models.TipPachet.Cutie, // Presupunand ca ai pastrat Enum-ul
//            nume_expeditor = "Expeditor Test SRL",
//            adresa_expeditor = "Str. Lalelelor 1, Bucuresti",
//            nume_destinatar = "Client Test",
//            adresa_destinatar = "Str. Trandafirilor 2, Cluj",
//            greutate_teoretica = 2.5m,
//            greutate_efectiva = 2.5m,
//            status_pachet = PacheteAPP.Models.StatusPachet.Inregistrat
//        };
//        db.Pachete.Add(pachetTest);
//        db.SaveChanges(); // Salvam pentru a obtine id_pachet
//        Console.WriteLine($"> Pachet creat cu succes. ID Pachet: {pachetTest.id_pachet}");

//        // 3. Cream Inregistrarea (Hub-ul care leaga actiunea)
//        var inregistrareTest = new PacheteAPP.Models.Inregistrare
//        {
//            tip_inregistrare = TipInregistrare.Deteriorare, // Aici poti ajusta daca ai Enum
//            id_user = testUser.Id,            // Preluam ID-ul curierului de la Pasul 1
//            id_pachet = pachetTest.id_pachet  // Preluam ID-ul pachetului proaspat creat
//        };
//        db.Inregistrari.Add(inregistrareTest);
//        db.SaveChanges(); // Salvam pentru a obtine id_inregistrare
//        Console.WriteLine($"> Inregistrare creata. ID Inregistrare: {inregistrareTest.id_inregistrare}");

//        // 4. Cream Extensia (Deteriorarea propriu-zisa)
//        var deteriorareTest = new PacheteAPP.Models.Deteriorare
//        {
//            locatie_deteriorare = "Hub Central Bucuresti",
//            descriere_deteriorare = "Cutia este indoita la coltul din stanga sus.",
//            id_inregistrare = inregistrareTest.id_inregistrare // Legam de hub-ul central
//        };
//        db.Deteriorari.Add(deteriorareTest);
//        db.SaveChanges();
//Console.WriteLine("> Deteriorare adaugata cu succes!");

//Console.WriteLine("--- TEST FINALIZAT CU SUCCES! BAZA DE DATE FUNCTIONEAZA PERFECT ---");
//    }
//    catch (Exception ex)
//    {
//    Console.WriteLine($"\n[!!!] EROARE FATALA LA TEST: {ex.Message}");
//    if (ex.InnerException != null)
//    {
//        Console.WriteLine($"[!!!] DETALII SQL: {ex.InnerException.Message}\n");
//    }
//}
//}
//// -------------------------------------------





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
