using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Api.ViewModels.Ticket {
    public class TicketOptionsViewModel {
        public long? AgentId { get; set; }

        public long DepartmentId { get; set; }

        public long StatusId { get; set; }
    }
}
