using System.ComponentModel.DataAnnotations.Schema;

namespace Uptime.Data.Models {
    public class Theme {
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
