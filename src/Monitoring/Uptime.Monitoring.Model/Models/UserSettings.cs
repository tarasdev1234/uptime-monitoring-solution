using System.ComponentModel.DataAnnotations;

namespace Uptime.Monitoring.Model.Models
{
    public class UserSettings {
        [Key]
        public long UserId { get; set; }
        
        public bool InformFeatures { get; set; }

        public bool InformDev { get; set; }

        public string CurrentTimeZone { get; set; }
    }
}
