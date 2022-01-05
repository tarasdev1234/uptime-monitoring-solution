using System.Collections.Generic;
using Uptime.Data.Models.Branding;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uptime.Data.Models.Billing {
    public class ProductGroup {
        public long Id { get; set; }

        public string Name { get; set; }

        public long BrandId { get; set; }

        public Brand Brand { get; set; }

        public List<Product> Products { get; set; }

        [NotMapped]
        public int ProductsCount { get; set; }

        [NotMapped]
        public string BrandName { get; set; }
    }
}
