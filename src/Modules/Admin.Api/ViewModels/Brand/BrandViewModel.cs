using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Api.ViewModels.Brands {
    public class BrandViewModel {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Theme { get; set; }

        public string Url { get; set; }

        public List<string> Plugins { get; set; }
    }
}
