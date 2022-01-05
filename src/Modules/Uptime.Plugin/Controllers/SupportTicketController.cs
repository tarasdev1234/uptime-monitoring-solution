using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Uptime.Mvc.Controllers;
using Uptime.Plugin.ViewModels.Support;
using Uptime.Data;
using Uptime.Monitoring.Data;
using AutoMapper;
using System.Net;
using Uptime.Monitoring.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Reliablesite.Authority.Authentication;
using Uptime.Data.Models.Support;
using Uptime.Plugin.Services;


namespace Uptime.Plugin.Controllers
{

    [Authorize]
    [Microsoft.AspNetCore.Mvc.Route("/api/support/tickets")]
    public class SupportTicketController : BaseController
    {
        private readonly UptimeMonitoringContext _monitoringContext;
       
        private readonly ILogger logger;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public SupportTicketController
            (ApplicationDbContext ctx,
            UptimeMonitoringContext monitoringContext,
            ILogger<SupportTicketController> logger,
            IMapper mapper,
            IWebHostEnvironment hostingEnvironment

            ) : base(ctx)
        {
           
            _monitoringContext = monitoringContext;
            this.logger = logger;
           _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }

     
        public async Task<IActionResult> Support()
        {

            return View();
        }

       

        [Microsoft.AspNetCore.Mvc.HttpPost("create")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
       // [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> CreateSupportTicket([FromForm] CreateSupportTicketViewModel vm)
        {
            SupportTicket tckt = null;
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                string uniqueFileName = string.Empty;

                if (vm.File != null)
                {
                    string uploadedFolder = Path.Combine(_hostingEnvironment.WebRootPath, "files");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + vm.File.FileName;
                    string filePath = Path.Combine(uploadedFolder, uniqueFileName);
                    vm.File.CopyTo(new FileStream(filePath, FileMode.Create));

                }

                var userId = User.GetId();
                tckt = new SupportTicket()
                {

                    Subject = vm.Subject,
                    Email = vm.Email,
                    Message = vm.Message,
                    FileName = uniqueFileName

                };

                _monitoringContext.SupportTicket.Add(tckt);
                await _monitoringContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest(e.Message);
            }

            return Ok(tckt);
           
        }

    }

}
