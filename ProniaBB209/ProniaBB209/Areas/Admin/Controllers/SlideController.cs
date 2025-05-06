using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBB209.DAL;
using ProniaBB209.Models;

namespace ProniaBB209.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _context.Slides.ToListAsync();
            return View(slides);
        }
        //public string Test()
        //{ 
        //ahsjgdjhsagdjagsdjhasg.jpg
        //    //hmsjavahsvd-amsbdabmdsaflowers.jpg
        //    //.jpg
         // flowers.jpgjhgamgdamsdgmashdgamdsmsagd
        //    return Guid.NewGuid().ToString();
        //}
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Slide slide)
        {


            if (!slide.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError(nameof(Slide.Photo), "File type is incorrect");
                return View();
            }

            if (slide.Photo.Length > 1 * 1024*1024)
            {
                ModelState.AddModelError(nameof(Slide.Photo), "File size shoul be less than 2MB");
                return View();
            }
 
             string fileName =string.Concat(Guid.NewGuid().ToString(), slide.Photo.FileName);
            //flowers.png
            string path = Path.Combine(_env.WebRootPath, "assets","images","website-images",fileName);

            FileStream fl = new FileStream(path,FileMode.Create);
            await slide.Photo.CopyToAsync(fl);

            slide.Image = fileName;

            slide.CreatedAt = DateTime.Now;
            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();




            return RedirectToAction(nameof(Index));











            //if (!ModelState.IsValid) return View();










        }
    }
}
