using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaBB209.DAL;
using ProniaBB209.Models;
using ProniaBB209.Utilities.Enums;
using ProniaBB209.Utilities.Extensions;
using ProniaBB209.ViewModels;

namespace ProniaBB209.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles ="Admin,Moderator")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<GetProductVM> productVMs = await _context.Products.Select(p => new GetProductVM
            {
                Name = p.Name,
                SKU = p.SKU,
                Id = p.Id,
                Price = p.Price,
                CategoryName = p.Category.Name,
                MainImage = p.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true).Image
            }).ToListAsync();
            return View(productVMs);
        }


        public async Task<IActionResult> Create()
        {

            CreateProductVM productVM = new CreateProductVM
            {
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync()
            };

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(productVM);
            }

            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(CreateProductVM.CategoryId), "Category does not exist");
                return View(productVM);
            }



            if (productVM.TagIds is null)
            {
                productVM.TagIds = new();
            }
            bool tagResult = productVM.TagIds
                                 .Any(tagId => !productVM.Tags.Exists(t => t.Id == tagId));
            if (tagResult)
            {
                ModelState.AddModelError(nameof(CreateProductVM.TagIds), "Tag id is wrong");
                return View(productVM);
            }


            if (!productVM.MainPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateProductVM.MainPhoto), "File type is incorrect");
                return View(productVM);
            }
            if (!productVM.MainPhoto.ValidateSize(FileSize.KB, 500))
            {
                ModelState.AddModelError(nameof(CreateProductVM.MainPhoto), "File size must be less than 500 KB");
                return View(productVM);
            }

            if (!productVM.SecondaryPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateProductVM.SecondaryPhoto), "File type is incorrect");
                return View(productVM);
            }
            if (!productVM.SecondaryPhoto.ValidateSize(FileSize.KB, 500))
            {
                ModelState.AddModelError(nameof(CreateProductVM.SecondaryPhoto), "File size must be less than 500 KB");
                return View(productVM);
            }


            bool nameResult = await _context.Products.AnyAsync(p => p.Name == productVM.Name);
            if (nameResult)
            {
                ModelState.AddModelError(nameof(CreateProductVM.Name), $"Product with {productVM.Name} already exists");
                return View(productVM);
            }

            ProductImage main = new ProductImage
            {
                Image = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                IsPrimary = true,
                CreatedAt = DateTime.Now

            };
            ProductImage secondary = new ProductImage
            {
                Image = await productVM.SecondaryPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                IsPrimary = false,
                CreatedAt = DateTime.Now

            };
            Product product = new Product
            {
                Name = productVM.Name,
                Price = productVM.Price.Value,
                SKU = productVM.SKU,
                Description = productVM.Description,
                CategoryId = productVM.CategoryId.Value,
                ProductImages = new List<ProductImage> { main,secondary },
                ProductTags = productVM.TagIds.Select(tId => new ProductTag { TagId = tId }).ToList()
            };




            if(productVM.AdditionalPhotos is not null)
            {
                string text = string.Empty;
                foreach (IFormFile photo in productVM.AdditionalPhotos)
                {
                    if (!photo.ValidateSize(FileSize.MB, 1))
                    {
                        text += $"<p>{photo.FileName} named image is oversized</p>";
                        continue;
                    }
                    if (!photo.ValidateType("image/"))
                    {
                        text += $"<p>{photo.FileName} named image type is incorrect</p>";
                        continue;
                    }
                   
                    product.ProductImages.Add(new ProductImage
                    {
                        Image = await photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                        IsPrimary = null,
                        CreatedAt = DateTime.Now

                    });

                }

                TempData["FileWarning"] = text;
            }





            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Product? product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductTags)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                SKU = product.SKU,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                PrimaryImage = product.ProductImages.FirstOrDefault(p => p.IsPrimary == true).Image,
                SecondaryImage = product.ProductImages.FirstOrDefault(p => p.IsPrimary == false).Image,
                ProductImages = product.ProductImages.Where(pi => pi.IsPrimary == null).ToList(),
                TagIds = product.ProductTags.Select(pt => pt.TagId).ToList(),
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync()
            };
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdateProductVM productVM)
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(productVM);
            }
            if (productVM.MainPhoto is not null)
            {

                if (!productVM.MainPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.MainPhoto), "File type is incorrect");
                    return View(productVM);
                }
                if (!productVM.MainPhoto.ValidateSize(FileSize.KB, 500))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.MainPhoto), "File size must be less than 500 KB");
                    return View(productVM);
                }

            }
            if (productVM.SecondaryPhoto is not null)
            {

                if (!productVM.SecondaryPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.SecondaryPhoto), "File type is incorrect");
                    return View(productVM);
                }
                if (!productVM.SecondaryPhoto.ValidateSize(FileSize.KB, 500))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.SecondaryPhoto), "File size must be less than 500 KB");
                    return View(productVM);
                }

            }
            bool result = productVM.Categories.Any(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(UpdateProductVM.CategoryId), "Category does not exist");
                return View(productVM);
            }
            if(productVM.TagIds is null)
            {
                productVM.TagIds = new();
            }

            productVM.TagIds = productVM.TagIds.Distinct().ToList();
            bool tagResult = productVM.TagIds.Any(tId => !productVM.Tags.Exists(t => t.Id == tId));
            if (tagResult)
            {
                ModelState.AddModelError(nameof(UpdateProductVM.TagIds), "Tag does not exist");
                return View(productVM);
            }

            bool nameResult = await _context.Products.AnyAsync(p => p.Name == productVM.Name && p.Id != id);
            if (nameResult)
            {
                ModelState.AddModelError(nameof(UpdateProductVM.Name), "Prodcut already exist");
                return View(productVM);
            }


            Product? existed = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductTags)
                .FirstOrDefaultAsync(p => p.Id == id);



            
            _context.ProductTags
                .RemoveRange(existed.ProductTags
                    .Where(pt => !productVM.TagIds
                        .Exists(tId => tId == pt.TagId)));


           _context.ProductTags
                .AddRange(productVM.TagIds
                    .Where(tId => !existed.ProductTags.Exists(pt => pt.TagId == tId))
                    .Select(tId => new ProductTag
                    {
                        TagId = tId,
                        ProductId = existed.Id
                    }));


            //1 2 3 4    1 2 

            if(productVM.ImageIds is null)
            {
                productVM.ImageIds = new();
            }



          List<ProductImage> deletedImages=existed.ProductImages
                .Where(pi => !productVM.ImageIds.Exists(imgId => pi.Id == imgId)&&pi.IsPrimary==null)
                .ToList();

            deletedImages
                .ForEach(di => di.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images"));

            _context.ProductImages.RemoveRange(deletedImages);


            if (productVM.MainPhoto is not null)
            {
                ProductImage main = new ProductImage
                {
                    Image = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                    IsPrimary = true,
                    CreatedAt = DateTime.Now
                };
                ProductImage existedMain = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);

                existedMain.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");

                existed.ProductImages.Remove(existedMain);
                existed.ProductImages.Add(main);

            }

            if (productVM.SecondaryPhoto is not null)
            {
                ProductImage secondary = new ProductImage
                {
                    Image = await productVM.SecondaryPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                    IsPrimary = false,
                    CreatedAt = DateTime.Now
                };
                ProductImage existedSecondary = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == false);

                existedSecondary.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");

                existed.ProductImages.Remove(existedSecondary);
                existed.ProductImages.Add(secondary);

            }

            if (productVM.AdditionalPhotos is not null)
            {
                string text = string.Empty;
                foreach (IFormFile photo in productVM.AdditionalPhotos)
                {
                    if (!photo.ValidateSize(FileSize.MB, 1))
                    {
                        text += $"<p>{photo.FileName} named image is oversized</p>";
                        continue;
                    }
                    if (!photo.ValidateType("image/"))
                    {
                        text += $"<p>{photo.FileName} named image type is incorrect</p>";
                        continue;
                    }

                    existed.ProductImages.Add(new ProductImage
                    {
                        Image = await photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                        IsPrimary = null,
                        CreatedAt = DateTime.Now

                    });

                }

                TempData["FileWarning"] = text;
            }

            existed.Name = productVM.Name;
            existed.Price = productVM.Price.Value;
            existed.Description = productVM.Description;
            existed.CategoryId = productVM.CategoryId.Value;
            existed.SKU = productVM.SKU;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id <= 0) return BadRequest();

            Product? product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

            foreach (ProductImage proImage in product.ProductImages)
            {
                proImage.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
