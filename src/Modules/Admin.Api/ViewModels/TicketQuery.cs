using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Api.ViewModels {
    public class TicketQuery {
        public bool? Queue { get; set; }

        public bool? Open { get; set; }

        public bool? Active { get; set; }

        public bool? Global { get; set; }
    }
}
