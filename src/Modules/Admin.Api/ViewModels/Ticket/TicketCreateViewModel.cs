using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Api.ViewModels.Ticket {
    public class TicketCreateViewModel {
        [Required]
        public long DepartmentId { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
