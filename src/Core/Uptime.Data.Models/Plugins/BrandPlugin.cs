using Newtonsoft.Json;
using Uptime.Data.Models.Branding;

namespace Uptime.Data.Models.Plugins {
    public class BrandPlugin {
        public long BrandId { get; set; }

        [JsonIgnore]
        public Brand Brand { get; set; }
        
        public string Name { get; set; }
    }
}
