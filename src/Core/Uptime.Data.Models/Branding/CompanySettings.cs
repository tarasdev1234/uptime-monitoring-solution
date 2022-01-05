using Uptime.Data.Models.Identity;

namespace Uptime.Data.Models.Branding {
    public class CompanySettings {
        public long CompanyId { get; set; }

        public Company Company { get; set; }

        public long UserId { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsOwner { get; set; }
    }
}
