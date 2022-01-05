using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Web.Mvc;



namespace Uptime.Monitoring.Model.Models
{
    public class SupportTicket
    {
        [Key]
        public long DbSupportID { get; set; }

        [NotMapped]
        public virtual SelectList Department { get; set; }
        
        [Required]
        public string Email { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string FileName { get; set; }
       
    }
}
