using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Uptime.Data.Models.Identity {
    public class ApplicationRole : IdentityRole<long> {
        public enum RoleType {
            User,
            Staff
        }
        
        public ApplicationRole () { }

        public ApplicationRole (string roleName) : base(roleName) { }

        [JsonIgnore]
        public List<RoleClaim> Claims { get; set; }

        [JsonIgnore]
        public override string ConcurrencyStamp { get; set; }

        [JsonIgnore]
        public override string NormalizedName { get; set; }

        public RoleType Type { get; set; }
    }
}
