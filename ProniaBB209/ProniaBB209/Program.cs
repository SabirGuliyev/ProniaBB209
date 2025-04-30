using Microsoft.EntityFrameworkCore;
using ProniaBB209.DAL;

//Singleton
//Scoped
//Transient
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer("server=msi;database=ProniaBB209;trusted_connection=true;integrated security=true;TrustServerCertificate=true;");
});

var app = builder.Build();

app.UseStaticFiles();


app.MapControllerRoute(
    "default",
    "{controller=home}/{action=index}/{id?}"
    );

app.Run();


