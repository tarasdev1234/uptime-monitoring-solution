using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Uptime.Data.Models.Identity;

namespace Uptime.Data.Models.Branding {
    public class Company {
        public long Id { get; set; }

        public long BrandId { get; set; }

        [JsonIgnore]
        public Brand Brand { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public List<CompanySettings> Users { get; set; }

        [NotMapped]
        public int UsersCount { get; set; }

        [NotMapped]
        public string BrandName { get; set; }
    }
}
