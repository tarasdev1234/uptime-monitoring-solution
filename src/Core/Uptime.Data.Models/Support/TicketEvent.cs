using System;

namespace Uptime.Data.Models.Support
{
    public class TicketEvent {
        public long Id { get; set; }

        public long TicketId { get; set; }

        public DateTime Date { get; set; }

        public long Type { get; set; }
    }
}
