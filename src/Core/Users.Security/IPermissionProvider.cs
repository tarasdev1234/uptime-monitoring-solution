using System;
using System.Collections.Generic;

namespace Users.Security
{
    public interface IPermissionProvider {
        IEnumerable<Permission> GetPermissions ();
    }
}
