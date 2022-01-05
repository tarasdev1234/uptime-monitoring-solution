using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uptime.Data.Models.Billing {
    public class Product {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public long? ProductGroupId { get; set; }

        [JsonIgnore]
        public ProductGroup ProductGroup { get; set; }

        [NotMapped]
        public string ProductGroupName { get; set; }
    }
}
