using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Api.ViewModels.Ticket {
    public class TicketSummaryViewModel {
        public int Queue { get; set; }

        public int Active { get; set; }

        public int Waiting { get; set; }

        public int ActiveGlobal { get; set; }

        public int WaitingGlobal { get; set; }
    }
}
