using Microsoft.AspNetCore.Identity;

namespace BackEnd.Entities
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }
    }
}
