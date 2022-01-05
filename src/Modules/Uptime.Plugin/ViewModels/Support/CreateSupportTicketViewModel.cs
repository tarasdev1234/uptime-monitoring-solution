using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;

namespace Uptime.Plugin.ViewModels.Support
{
    public class CreateSupportTicketViewModel
    {

        public long Id { get; set; }
        
        public virtual SelectList Department { get; set; }
      
        [Required]
        public string Email { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
        
        [Required]
        public IFormFile File { get; set; }
    }
}
