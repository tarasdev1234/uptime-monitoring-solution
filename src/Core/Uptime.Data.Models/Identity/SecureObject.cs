using System.Collections.Generic;

namespace Uptime.Data.Models.Identity {
    public class SecureObject {
        public const long KB_ARTICLES = 100;
        
        public const long KB_COMMENTS = 200;

        public const long USERS = 300;

        public const long TICKETS = 400;

        public long Id { get; set; }

        public string Name { get; set; }

        public List<PermissionType> Permissions { get; set; }
    }
}
