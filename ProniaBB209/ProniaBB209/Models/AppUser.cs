using Microsoft.AspNetCore.Identity;

namespace ProniaBB209.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }

    }
}
