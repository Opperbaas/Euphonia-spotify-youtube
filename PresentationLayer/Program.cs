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
builder.Services.AddScoped<ISpecificService, SpecificService>();
builder.Services.AddScoped<IMuziekService, MuziekService>();

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
app.UseAuthorization();

// Routes - MVC EERST!
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers(); // API routes

app.Run();
