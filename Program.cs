global using PacheteAPP.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PacheteAPP.Data;
using PacheteAPP.Services;
using PacheteAPP.Services.CreateDeteriorare;
using PacheteAPP.Services.CreateInfoLipsa;
using PacheteAPP.Services.CreatePachet;
using PacheteAPP.Services.Imagini;
using PacheteAPP.Services.Login;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//API
builder.Services.AddScoped<IPachetService, PachetService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDeteriorareService, DeteriorareService>(); // pff aici era problema
builder.Services.AddScoped<IInfoLipsaService, InfoLipsaService>();
builder.Services.AddScoped<IImagineService, ImagineService>();

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
});

builder.Services.AddControllers();

builder.Services.AddQuickGridEntityFrameworkAdapter();  
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// modificare db
builder.Services.AddRazorPages();

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiUser", policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
    });
    options.AddPolicy("CanView", policy => policy.RequireClaim("Permisiune", "View"));
    options.AddPolicy("CanCreate", policy =>
    {
        policy.AddAuthenticationSchemes(
            IdentityConstants.ApplicationScheme,
            JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser();
        policy.RequireClaim("Permisiune", "Create");
    });
    options.AddPolicy("CanEdit", policy => policy.RequireClaim("Permisiune", "Edit"));
    options.AddPolicy("CanDelete", policy => policy.RequireClaim("Permisiune", "Delete"));
    options.AddPolicy("CanUseMobileApp", policy => policy.RequireClaim("Permisiune", "UseMobileApp"));
});

var app = builder.Build();

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

// Only apply the HTML error page for non-API requests.
// API routes return their own status codes; re-executing them as GET /not-found
// causes a 405 Method Not Allowed when the original method was POST/PUT/DELETE.
app.UseWhen(
    ctx => !ctx.Request.Path.StartsWithSegments("/api"),
    appBuilder => appBuilder.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true)
);

//app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorPages()
   .WithStaticAssets();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await PacheteAPP.Data.SeedData.InitializeAsync(services);
}
app.Run();
