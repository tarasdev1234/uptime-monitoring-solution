using Microsoft.AspNetCore.Identity;

namespace Uptime.Data.Models.Identity {
    public class UserRole : IdentityUserRole<long> {
        public ApplicationUser User { get; set; }

        public ApplicationRole Role { get; set; }
    }
}
