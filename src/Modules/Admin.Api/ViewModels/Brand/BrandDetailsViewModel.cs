using System;
using System.Collections.Generic;
using System.Text;
using Uptime.Data.Models.Branding;

namespace Admin.Api.ViewModels.Brands {
    public class BrandDetailsViewModel {
        public Brand Brand { get; set; }

        public List<string> Plugins { get; set; }
    }
}
