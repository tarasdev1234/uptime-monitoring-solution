using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Uptime.Plugin.ViewModels.Dashboard {
    public class CreateMonitorViewModel {
        public long Id { get; set; }

        [Required]
        public int Interval { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public int AuthType { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Url { get; set; }

        public string HttpUser { get; set; }

        public string HttpPassword { get; set; }

        [Required]
        public List<long> AlertContacts { get; set; }
    }
}
