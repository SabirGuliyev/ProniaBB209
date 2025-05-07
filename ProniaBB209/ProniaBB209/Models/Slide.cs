using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProniaBB209.Models
{
    public class Slide : BaseEntity
    {
        [MaxLength(200,ErrorMessage ="Slide title must be less than 200 characters")]
        public string Title { get; set; } 
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Order { get; set; }
    }

}
