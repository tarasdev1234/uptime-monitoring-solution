using Uptime.Data.Models.Support;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uptime.Data.Models.Branding {
    public class Department {
        public long Id { get; set; }

        public long BrandId { get; set; }

        public long? SmtpServerId { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public Brand Brand { get; set; }

        public bool ResponderEnabled { get; set; }

        public long? AutoResponderId { get; set; }

        [JsonIgnore]
        public AutoResponder AutoResponder { get; set; }

        [JsonIgnore]
        public List<UserDepartment> Users { get; set; }

        [JsonIgnore]
        public List<Ticket> Tickets { get; set; }

        [JsonIgnore]
        public SmtpServer SmtpServer { get; set; }

        [NotMapped]
        public string BrandName { get; set; }

        [NotMapped]
        public int UsersCount { get; set; }

        [NotMapped]
        public int TicketsCount { get; set; }
    }
}
