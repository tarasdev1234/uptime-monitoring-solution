using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uptime.Data.Models.Identity
{
    public class ApplicationUser : IdentityUser<long> {
        public bool IsEnabled { get; set; } = true;

        public string FullName { get; set; }

        public string Timezone { get; set; }

        public string Signature { get; set; }

        [JsonIgnore]
        public List<UserPermission> Permissions { get; set; }

        [JsonIgnore]
        public override string PasswordHash { get; set; }

        [JsonIgnore]
        public override string ConcurrencyStamp { get; set; }

        [JsonIgnore]
        public override string SecurityStamp { get; set; }

        [NotMapped]
        public string AvatarUrl { get; set; }
    }
}
