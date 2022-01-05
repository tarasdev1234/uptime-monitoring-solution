using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Reliablesite.Service.Core.Controllers
{
    [OpenApiIgnore]
    [Route("/health")]
    [AllowAnonymous]
    [ApiController]
    public sealed class HealthCheckController : ControllerBase
    {
        private readonly HealthCheckService healthCheckService;

        public HealthCheckController(HealthCheckService healthCheckService)
        {
            this.healthCheckService = healthCheckService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyDictionary<string, HealthReportEntry>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetAsync(CancellationToken token)
        {
            var report = await healthCheckService.CheckHealthAsync(token);
            var code = report.Status == HealthStatus.Healthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;

            return StatusCode(code, report.Entries);
        }
    }
}
