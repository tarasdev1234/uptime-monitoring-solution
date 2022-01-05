using System.Collections.Generic;
using System.Linq;
using Uptime.Data.Models;
using Uptime.Data;
using Uptime.Data.Abstractions.Repositories;

namespace Uptime.Data.Repositories {
    public class ThemeRepository : BaseRepository<Theme>, IThemeRepository {
        public ThemeRepository (ApplicationDbContext dbContext) : base(dbContext) { }

        public IEnumerable<Theme> GetAll () {
            return dbSet.ToList();
        }
    }
}
