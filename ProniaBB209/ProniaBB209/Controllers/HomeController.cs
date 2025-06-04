using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBB209.DAL;
using ProniaBB209.Models;
using ProniaBB209.ViewModels;

namespace ProniaBB209.Controllers
{
    public class HomeController : Controller
    {
        //DI - Dependency Injection /Pattern
        //IOC - Inverse of Control /Principle
        //Service LifeTime
        //IOC Container/ DI Container
        //DIP - Dependency Inversion Principle

        public readonly AppDbContext _context;
    

        public HomeController(AppDbContext context)
        {
            _context = context;

           
        }
        public async Task<IActionResult> Index()
        {

           

            HomeVM homeVM = new HomeVM {
            Slides=await _context.Slides
            .OrderBy(s => s.Order)
            .Take(2)
            .ToListAsync(),

            Products=await _context.Products
            .Take(8)
            .Include(p => p.ProductImages.Where(pi=>pi.IsPrimary!=null))
            .ToListAsync()
            };

            return View(homeVM);
        }


        public IActionResult Error(string errorMessage)
        {
            return View(model:errorMessage);
        }
    }
}
