using System;
using System.Collections.Generic;
using System.Text;
using Uptime.Data.Models.Branding;
using Uptime.Data;

namespace Uptime.Data.Abstractions.Repositories.Branding {
    public interface IBrandRepository : IBaseRepository<Brand> {
        IEnumerable<Brand> GetAll ();
    }
}
