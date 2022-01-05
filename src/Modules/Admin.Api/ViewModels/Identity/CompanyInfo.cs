using Admin.Api.ViewModels.Brands;
using System.Collections.Generic;

namespace Admin.Api.ViewModels.Identity
{
    public class CompanyInfo
    {
        public CompanySettingsViewModel Company { get; set; }
        public List<long> Departments { get; set; }
    }
}
