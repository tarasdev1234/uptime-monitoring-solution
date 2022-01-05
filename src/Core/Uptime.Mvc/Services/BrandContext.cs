using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OrchardCore.Environment.Shell;
using System.Linq;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Data.Models.Branding;

namespace Uptime.Mvc.Services {
    public class BrandContext {
        private readonly ShellSettings _shellSettings;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ApplicationDbContext dbContext;
        private Brand brand = null;

        public BrandContext (IHttpContextAccessor httpContextAccessor,
            ShellSettings shellSettings,
            ApplicationDbContext dbContext) {
            _shellSettings = shellSettings;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }

        public async Task<Brand> GetCurrentBrandAsync () {
            if (brand == null) {
                brand = await dbContext.Brands
                    .Include(b => b.BrandPlugins)
                    .Where(b => b.Url == httpContextAccessor.HttpContext.Request.Host.Host)
                    //.Where(b => b.Name == _shellSettings.Name)
                    .FirstOrDefaultAsync();
            }

            return brand;
        }
    }
}
