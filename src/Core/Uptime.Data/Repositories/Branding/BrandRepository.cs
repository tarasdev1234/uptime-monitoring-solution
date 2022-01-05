using System.Collections.Generic;
using System.Linq;
using Uptime.Data.Abstractions.Repositories.Branding;
using Uptime.Data.Models.Branding;
using Uptime.Data;

namespace Uptime.Data.Repositories.Branding {
    public class BrandRepository : BaseRepository<Brand>, IBrandRepository {
        public BrandRepository (ApplicationDbContext dbContext) : base(dbContext) { }

        public IEnumerable<Brand> GetAll () {
            return dbSet.ToList();
        }
    }
}
