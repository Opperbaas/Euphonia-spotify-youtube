using Microsoft.EntityFrameworkCore;
using Euphonia.DataAccessLayer.Context;
using Euphonia.DataAccessLayer.Interfaces;
using Euphonia.DataAccessLayer.UnitOfWork;
using Euphonia.BusinessLogicLayer.Interfaces;
using Euphonia.BusinessLogicLayer.Services;

var builder = WebApplication.CreateBuilder(args);

// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Euphonia.DataAccessLayer")
    )
);

// Dependency Injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMuziekService, MuziekService>();
builder.Services.AddScoped<IProfielService, ProfielService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStemmingService, StemmingService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// External Music APIs
builder.Services.AddScoped<ISpotifyService, SpotifyService>();
builder.Services.AddScoped<IYouTubeService, YouTubeService>();

// Session configuratie
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// MVC + API Controllers
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.RoutePrefix = "swagger");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// Fallback voor root EN index.html naar Home
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower() ?? "";
    if (path == "/" || path == "/index.html")
    {
        context.Response.Redirect("/Home/Index");
        return;
    }
    await next();
});

// Routes - MVC EERST!
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers(); // API routes

app.Run();
