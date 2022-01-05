using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Uptime.Notifications.Model.Abstractions;
using Uptime.Notifications.Model.Models;
using Uptime.Notifications.Web.Dto;

namespace Uptime.Notifications.Web.Controllers
{
    [Route(Routes.TemplatesV0)]
    [ApiController]
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplatesService templatesService;
        private readonly IMapper mapper;

        public TemplatesController(ITemplatesService templatesService, IMapper mapper)
        {
            this.templatesService = templatesService;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(TemplateShort[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTemplates(string scope, string name, CancellationToken token)
        {
            var templates = templatesService.FindTemplates(new TemplateQuery { Name = name, Scope = scope });

            return Ok(await mapper.ProjectTo<TemplateShort>(templates).ToListAsync());
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TemplateFull), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTemplate(Guid id, CancellationToken token)
        {
            var template = await templatesService.GetTemplateAsync(id, token);

            return Ok(mapper.Map<TemplateFull>(template));
        }

        [HttpPost]
        public async Task<IActionResult> AddTemplate(TemplateFull template, CancellationToken token)
        {
            var mapped = mapper.Map<Template>(template);
            await templatesService.AddTemplateAsync(mapped, token);

            return CreatedAtAction(nameof(GetTemplate), new { id = mapped.Id }, mapper.Map<TemplateShort>(mapped));
        }
    }
}
