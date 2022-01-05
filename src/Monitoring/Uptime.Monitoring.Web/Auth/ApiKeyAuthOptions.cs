using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;

namespace Uptime.Monitoring.Auth {
    public class ApiKeyAuthOptions : AuthenticationSchemeOptions {
        public static readonly string DefaultScheme = "ApiKey";

        public StringValues ApiKeys { get; set; }
    }
}
