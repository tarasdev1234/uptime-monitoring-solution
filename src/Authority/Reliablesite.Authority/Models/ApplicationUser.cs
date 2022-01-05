using Microsoft.AspNetCore.Identity;

namespace Reliablesite.Authority.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<long>
    {
        public string Signature { get; set; }
    }
}
