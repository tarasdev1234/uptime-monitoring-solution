using System.ComponentModel.DataAnnotations.Schema;

namespace Uptime.Data.Models.Support {
    public class AutoResponder {
        public long Id { get; set; }

        [NotMapped]
        public bool IsEnabled { get; set; }

        public string Template { get; set; }
    }
}
