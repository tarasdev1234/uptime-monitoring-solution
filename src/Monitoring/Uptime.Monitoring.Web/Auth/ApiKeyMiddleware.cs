using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Threading.Tasks;

namespace Uptime.Monitoring.Auth
{
    public class ApiKeyMiddleware {
        public const string API_KEY_HEADER_NAME = "X-SERVER-API-KEY";
        public IConfiguration Configuration;

        private readonly RequestDelegate next;

        public ApiKeyMiddleware (RequestDelegate next, IConfiguration config) {
            Configuration = config;
            this.next = next;
        }

        public async Task Invoke (HttpContext context) {
            if (!context.Request.Path.StartsWithSegments("/api")
                || context.Request.Path.StartsWithSegments("/api/internal")) {
                await next.Invoke(context);
                return;
            }

            var apiKey = context.Request.Headers[API_KEY_HEADER_NAME];
            if (apiKey == StringValues.Empty) {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync($"Header {API_KEY_HEADER_NAME} is not presented in request");
                return;
            } else {
                if (!string.Equals(apiKey.FirstOrDefault(), Configuration["APIKEY"])) {
                    context.Response.StatusCode = 401;
                    return;
                }
            }

            await next.Invoke(context);
        }
    }
}
