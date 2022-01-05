using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace Uptime.Monitoring.Web.Controllers.Internal
{
    [Route("/api/internal/logs")]
    public class LogsInternalController : ControllerBase
    {
        public LogsInternalController()
        {
        }

        [HttpGet("{monitorId:long}")]
        public ActionResult GetLogs(long monitorId)
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", $"monitor-{monitorId}", $"monitor-{monitorId}.log");

            if (System.IO.File.Exists(logPath))
            {
                return PhysicalFile(logPath, "text/plain");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
