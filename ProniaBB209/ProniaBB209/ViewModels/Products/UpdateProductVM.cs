using ProniaBB209.Models;
using System.ComponentModel.DataAnnotations;

namespace ProniaBB209.ViewModels
{
    public class UpdateProductVM
    {

        public string Name { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }

        [Required]
        public int? CategoryId { get; set; }
        public List<int>? TagIds { get; set; }
        public List<int>? ImageIds { get; set; }

        public string PrimaryImage { get; set; }
        public string SecondaryImage { get; set; }

        public List<ProductImage>? ProductImages { get; set; }
        public IFormFile? MainPhoto { get; set; }
        public IFormFile? SecondaryPhoto { get; set; }
        public List<IFormFile>? AdditionalPhotos { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Tag>? Tags { get; set; }
    }
}
