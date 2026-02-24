using Microsoft.AspNetCore.Identity;

namespace Domain.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}