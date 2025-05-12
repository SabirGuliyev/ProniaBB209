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
        public string PrimaryImage { get; set; }
        public IFormFile? MainPhoto { get; set; }
        public List<Category>? Categories { get; set; }
    }
}
