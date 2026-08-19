using KLHealth.Web.Data;
using KLHealth.Web.Services;
using KLHealth.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar servicios de MVC
builder.Services.AddControllersWithViews();

// 2. Configurar Entity Framework Core
builder.Services.AddDbContext<KLHealthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Configurar Autenticación por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// 4. Registrar IHttpContextAccessor (necesario para el AuthService)
builder.Services.AddHttpContextAccessor();

// 5. Registrar nuestros servicios personalizados
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// 6. Sembrar datos iniciales
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<KLHealthDbContext>();
    DbInitializer.Initialize(context);
}

// 7. Configurar el pipeline de HTTP
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ¡IMPORTANTE! El orden importa: Routing -> Authentication -> Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();