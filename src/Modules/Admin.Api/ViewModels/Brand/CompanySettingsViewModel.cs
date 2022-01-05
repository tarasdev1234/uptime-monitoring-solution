using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Api.ViewModels.Brands {
    public class CompanySettingsViewModel {
        public long? CompanyId { get; set; }

        public bool? IsAdmin { get; set; } = false;

        public bool? IsOwner { get; set; } = false;
    }
}
