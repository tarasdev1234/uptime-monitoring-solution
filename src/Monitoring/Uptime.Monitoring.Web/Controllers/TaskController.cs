using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Uptime.Monitoring.Model.Abstractions;

namespace Uptime.Monitoring.Controllers
{
    [Route("/api/tasks")]
    [ApiController]
    public class TaskController : ControllerBase {
        private readonly IMonitoringTaskService monitoringTaskService;

        private ILogger logger { get; set; }

        public TaskController (
            IMonitoringTaskService monitoringTaskService,
            ILogger<TaskController> logger
        ) {
            this.monitoringTaskService = monitoringTaskService;
            this.logger = logger;
        }
        
        [HttpPost("{id}/start")]
        public async Task<ActionResult> Start (int id) {
            await monitoringTaskService.StartAsync(id);

            if (ModelState.ErrorCount > 0) {
                return BadRequest(new {
                    error = ModelState["error"]
                });
            }

            return Ok();
        }

        [HttpDelete("{id}/stop")]
        public async Task<ActionResult> Stop (int id) {
            await monitoringTaskService.StopAsync(id);

            return Ok(ModelState);
        }

        [HttpPost("{id}/pause")]
        public async Task<ActionResult> Pause (int id) {
            await monitoringTaskService.PauseAsync(id);

            return Ok(ModelState);
        }

        [HttpGet("stats")]
        public ActionResult Stats (int id) {
            return Ok(new {
                RunningTasks = monitoringTaskService.GetRunningIds().Count
            });
        }

        private void ErrorReport (string key, string message) {
            ModelState.AddModelError(key, message);
        }
    }
}
