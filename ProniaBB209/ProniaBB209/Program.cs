using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProniaBB209.DAL;
using ProniaBB209.Models;

//Singleton
//Scoped
//Transient
var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllersWithViews();

builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequiredLength = 8;
    opt.Password.RequireNonAlphanumeric = false;

    //opt.User.AllowedUserNameCharacters="qwertyuiopasdfghjkzxcvbnm1234567890_"
    opt.User.RequireUniqueEmail = true;

    opt.Lockout.MaxFailedAccessAttempts = 3;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


builder.Services.AddDbContext<AppDbContext>(opt =>
{
    //opt.UseSqlServer(builder.Configuration["ConnectionStrings:Default"]);
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

var app = builder.Build();

app.UseStaticFiles();

app.MapControllerRoute(
    "admin",
    "{area:exists}/{controller=home}/{action=index}/{id?}"
    );
app.MapControllerRoute(
    "default",
    "{controller=home}/{action=index}/{id?}"
    );

app.Run();


