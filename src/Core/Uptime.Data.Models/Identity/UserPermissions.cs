using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uptime.Data.Models.Identity {
    public class UserPermission {
        public long Id { get; set; }

        public long UserId { get; set; }

        public ApplicationUser User { get; set; }

        public long PermissionTypeId { get; set; }

        public PermissionType PermissionType { get; set; }

        public bool Allow { get; set; }
    }
}
