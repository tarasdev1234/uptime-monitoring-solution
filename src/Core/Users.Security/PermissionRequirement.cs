using Microsoft.AspNetCore.Authorization;
using System;

namespace Users.Security {
    public class PermissionRequirement : IAuthorizationRequirement {
        public PermissionRequirement (Permission permission) {
            Permission = permission ?? throw new ArgumentNullException(nameof(permission));
        }

        public Permission Permission { get; set; }
    }
}
