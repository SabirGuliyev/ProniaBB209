using System.ComponentModel.DataAnnotations;

namespace ProniaBB209.Models
{
    public class Category:BaseEntity
    {
        //[MinLength(3,ErrorMessage ="3den qisa ola bilmezzzzz")]
        [MaxLength(30)]
        public string Name { get; set; }

        //relational

        public List<Product>? Products { get; set; }
    }
}
