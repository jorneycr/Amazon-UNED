using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
    options.HttpsPort = 5001; // Puerto HTTPS que configuraste
});

// Configura el DbContext para usar SQL Server
builder.Services.AddDbContext<AmazonContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionLocal")));

// Configura Identity con roles
builder.Services.AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<AmazonContext>()
    .AddDefaultTokenProviders();

// Configura servicios adicionales
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed data e inicializar roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Seed roles "Usuario" y "Admin"
    AmazonContext.SeedRolesAsync(services).Wait();

    // Seed productos
    var context = services.GetRequiredService<AmazonContext>();
    await AmazonContext.SeedProductosAsync(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Asegúrate de agregar autenticación antes de la autorización
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
