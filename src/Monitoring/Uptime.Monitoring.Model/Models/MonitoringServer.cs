using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Uptime.Monitoring.Model.Validation;

namespace Uptime.Monitoring.Model.Models {
    public class MonitoringServer {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string AccessKey { get; set; }

        [Required]
        [RegexCheck(
            @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$",
            @"^(([a-zA-Z]|[a-zA-Z][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z]|[A-Za-z][A-Za-z0-9\-]*[A-Za-z0-9])$",
            ErrorMessage = "Host must be a valid ip address or hostname."
        )]
        public string Host { get; set; }

        [NotMapped]
        public int Health { get; set; }

        public int Status { get; set; }
    }
}
