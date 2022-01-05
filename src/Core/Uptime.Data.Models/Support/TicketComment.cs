using Uptime.Data.Models.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uptime.Data.Models.Support {
    public class TicketComment {
        public long Id { get; set; }

        public long TicketId { get; set; }

        [JsonIgnore]
        public Ticket Ticket { get; set; }

        public long CommentTypeId { get; set; }

        public CommentType CommentType { get; set; }

        public long UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }
    }
}
