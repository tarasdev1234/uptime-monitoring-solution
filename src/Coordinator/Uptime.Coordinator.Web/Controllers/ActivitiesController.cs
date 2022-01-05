using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Uptime.Coordinator.Model.Abstractions;
using Uptime.Coordinator.Model.Models;

namespace Uptime.Coordinator.Web.Controllers
{
    [Route(Routes.ActivitiesV0)]
    public sealed class ActivitiesController : ControllerBase
    {
        private readonly IActivitiesService activitiesService;

        public ActivitiesController(IActivitiesService activitiesService)
        {
            this.activitiesService = activitiesService;
        }

        [HttpGet("{executorId:guid}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<Activity>>> GetActivities(Guid executorId, CancellationToken token)
        {
            var activities = await activitiesService.GetActivitiesByExecutorAsync(executorId, token);

            return Ok(activities);
        }
    }
}
