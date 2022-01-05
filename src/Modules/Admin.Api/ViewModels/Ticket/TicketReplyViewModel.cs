using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Api.ViewModels.Ticket {
    public class TicketReplyViewModel {
        public long NewStatusId { get; set; }

        public string To { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }
    }
}
