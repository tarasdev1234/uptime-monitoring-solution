using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Api.ViewModels.Brands {
    public class DepartmentViewModel {
        public long Id { get; set; }

        public long BrandId { get; set; }

        public string Name { get; set; }

        public long SmtpServerId { get; set; }
    }
}
