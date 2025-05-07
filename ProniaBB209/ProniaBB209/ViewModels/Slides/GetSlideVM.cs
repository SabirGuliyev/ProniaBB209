using System.ComponentModel.DataAnnotations;

namespace ProniaBB209.ViewModels
{
    public class GetSlideVM
    {
        public int Id { get; set; }
        [MaxLength(200, ErrorMessage = "Slide title must be less than 200 characters")]
        public string Title { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
