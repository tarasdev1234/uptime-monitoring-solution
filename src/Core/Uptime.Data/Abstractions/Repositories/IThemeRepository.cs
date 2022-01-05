using System.Collections.Generic;
using Uptime.Data.Models;
using Uptime.Data;

namespace Uptime.Data.Abstractions.Repositories {
    public interface IThemeRepository : IBaseRepository<Theme> {
        IEnumerable<Theme> GetAll ();
    }
}
