using Microsoft.Extensions.Logging;
using RazorLight;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Uptime.Notifications.Email.Abstractions;
using Uptime.Notifications.Model.Models;

namespace Uptime.Notifications.Email.Services
{
    internal sealed class RazorTemplateEngine : IRazorTemplateEngine
    {
        private const string TemplatesLocation = "Templates";
        private readonly ILogger<RazorTemplateEngine> log;
        private static RazorLightEngine Engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(typeof(RazorTemplateEngine))
            .UseMemoryCachingProvider()
            .Build();

        public RazorTemplateEngine(ILogger<RazorTemplateEngine> log)
        {
            this.log = log;
        }

        private string GetFullTemplateName(Template template) => $"{TemplatesLocation}.{template.Scope}.{template.Name}";

        public async Task<string> RenderAsync(Template template, ExpandoObject data, CancellationToken token)
        {
            if (string.IsNullOrEmpty(template.Body))
            {
                log.LogDebug("Template {TemplateScope}.{TemplateName} does not contain body. Trying to render from embeded resources.", template.Scope, template.Name);
                return await Engine.CompileRenderAsync<object>(GetFullTemplateName(template), null, data);
            }
            else
            {
                return await Engine.CompileRenderStringAsync<object>(GetFullTemplateName(template), template.Body, null, data);
            }
        }
    }
}
