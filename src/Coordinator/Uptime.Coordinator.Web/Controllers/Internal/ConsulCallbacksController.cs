using Consul;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uptime.Coordinator.Model;
using Uptime.Coordinator.Model.Abstractions;

namespace Uptime.Coordinator.Web.Controllers.Internal
{
    [Route(Routes.ConsulCallbacks)]
    [ApiController]
    [AllowAnonymous]
    public class ConsulCallbacksController : ControllerBase
    {
        private readonly IActivitiesService activitiesService;

        public ConsulCallbacksController(IActivitiesService activitiesService)
        {
            this.activitiesService = activitiesService;
        }

        [HttpPut("services")]
        public async Task<IActionResult> ServicesWatchCallback(IReadOnlyList<ServiceEntry> services, CancellationToken token)
        {
            var activeServices = services
                .Where(x => x.Checks.All(chk => chk.Status == HealthStatus.Passing))
                .Select(x => ConsulHelpers.GetInstanceId(x.Service.ID));

            await activitiesService.CorrectLiveExecutorsAsync(activeServices, token);

            return Ok();
        }
    }
}
