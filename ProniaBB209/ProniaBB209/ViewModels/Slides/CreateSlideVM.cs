using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProniaBB209.ViewModels
{
    public class CreateSlideVM
    {

        [MaxLength(200, ErrorMessage = "Slide title must be less than 200 characters")]
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public IFormFile Photo { get; set; }
    }
}
