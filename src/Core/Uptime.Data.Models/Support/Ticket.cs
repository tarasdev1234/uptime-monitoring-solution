using Uptime.Data.Models.Branding;
using Uptime.Data.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Uptime.Data.Models.Support {
    public class Ticket {
        public long Id { get; set; }

        [NotMapped]
        public string DepartmentName { get; set; }

        [NotMapped]
        public string StatusName { get; set; }

        public string Subject { get; set; }

        public long? UserId { get; set; }

        public long? CustomerId { get; set; }

        public string CustomerEmail { get; set; }

        public Department Department { get; set; }

        public long DepartmentId { get; set; }

        [Required]
        public long StatusId { get; set; }

        public TicketStatus Status { get; set; }

        public bool IsDeleted { get; set; }
        
        public DateTime DateOpened { get; set; }

        public DateTime? DateClosed { get; set; }

        public List<TicketMessage> Messages { get; set; }

        public List<TicketEvent> Events { get; set; }

        public List<TicketComment> Comments { get; set; }
    }
}
