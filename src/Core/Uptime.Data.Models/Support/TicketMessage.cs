using Uptime.Data.Models.Identity;
using Newtonsoft.Json;
using System;

namespace Uptime.Data.Models.Support {
    public class TicketMessage {
        public enum MessageDirection {
            FromUser,
            FromSupport
        }

        public enum TicketMessageStatus {
            New,
            Delivered,
            Error
        }

        public long Id { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public DateTime Date { get; set; }

        public MessageDirection Direction { get; set; }

        public string SmtpMessageId { get; set; }

        public string Error { get; set; }

        public TicketMessageStatus Status { get; set; }

        public long? UserId { get; set; }

        public ApplicationUser User { get; set; }

        public long TicketId { get; set; }

        [JsonIgnore]
        public Ticket Ticket { get; set; }
    }
}
