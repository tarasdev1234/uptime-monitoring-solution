using System.Collections.Generic;
using Admin.Api.Constants;
using Users.Security;

namespace Admin.Api {
    public class Permissions : IPermissionProvider {
        public static readonly Permission ReadTickets = new Permission(Constants.Permissions.READ_TICKETS, "Read Tickets", "Tickets");
        public static readonly Permission DeleteTickets = new Permission(Constants.Permissions.DELETE_TICKETS, "Delete Tickets", "Tickets");

        public static readonly Permission ReadArticles = new Permission(Constants.Permissions.READ_ARTICLES, "Read Articles", "Knowledge Base");
        public static readonly Permission DeleteArticles = new Permission(Constants.Permissions.DELETE_ARTICLES, "Delete Articles", "Knowledge Base");

        public IEnumerable<Permission> GetPermissions () {
            return new[] {
                ReadTickets,
                DeleteTickets,
                ReadArticles,
                DeleteArticles
            };
        }
    }
}
