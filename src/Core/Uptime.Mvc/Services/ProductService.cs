using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Data.Models.Billing;
using Uptime.Mvc.Services;

namespace Uptime.Data.Services {
    public class ProductService {
        private readonly ApplicationDbContext dbCtx;
        private readonly BrandContext brandCtx;

        public ProductService (ApplicationDbContext dbCtx, BrandContext brandCtx) {
            this.dbCtx = dbCtx;
            this.brandCtx = brandCtx;
        }

        public async Task<List<ProductGroup>> GetGroups () {
            var brand = await brandCtx.GetCurrentBrandAsync();

            return await dbCtx.ProductGroups
                        .Where(pg => pg.BrandId == brand.Id)
                        .ToListAsync();
        }
    }
}
