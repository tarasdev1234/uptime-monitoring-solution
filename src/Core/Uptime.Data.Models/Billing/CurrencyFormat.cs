using Newtonsoft.Json;

namespace Uptime.Data.Models.Billing {
    public class CurrencyFormat {
        public long Id { get; set; }

        public string Format { get; set; }

        [JsonIgnore]
        public string Pattern { get; set; }

        public string Culture { get; set; }
    }
}
