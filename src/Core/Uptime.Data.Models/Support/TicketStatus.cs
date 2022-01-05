using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uptime.Data.Models.Support {
    public class TicketStatus {
        public const string OPEN = "open";
        public const string WAITING = "waiting";
        public const string CLOSED = "closed";
        public const string LOCKED = "open";

        public long Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public int Order { get; set; }

        public bool IsActive { get; set; }

        public bool IsOpen { get; set; }

        public bool IsLocked { get; set; }
    }
}
