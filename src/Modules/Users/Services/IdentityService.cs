using Microsoft.AspNetCore.Http;
using System;

namespace Users.Services {
    public class IdentityService : IIdentityService {
        private IHttpContextAccessor context;

        public IdentityService (IHttpContextAccessor context) {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public string GetUserIdentity () {
            return context.HttpContext.User.FindFirst("sub")?.Value;
        }
    }
}
