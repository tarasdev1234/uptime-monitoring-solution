using Newtonsoft.Json;
using Uptime.Data.Models.Branding;

namespace Uptime.Data.Models.Billing {
    public class Currency {
        public long Id { get; set; }

        public long? BrandId { get; set; }

        [JsonIgnore]
        public Brand Brand { get; set; }

        public string Code { get; set; }

        public string Prefix { get; set; }

        public string Suffix { get; set; }

        public float ConvertRate { get; set; }

        public bool Base { get; set; }

        public long FormatId { get; set; }

        public CurrencyFormat Format { get; set; }
    }
}
