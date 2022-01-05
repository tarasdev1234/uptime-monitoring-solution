using Microsoft.AspNetCore.Identity;

namespace Reliablesite.Authority.Models
{
    public class ApplicationRole : IdentityRole<long>
    {
        public ApplicationRole()
        {
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
        }
    }
}
