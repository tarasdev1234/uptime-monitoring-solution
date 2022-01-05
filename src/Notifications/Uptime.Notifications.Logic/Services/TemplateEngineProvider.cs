using Microsoft.Extensions.DependencyInjection;
using System;
using Uptime.Notifications.Email.Abstractions;
using Uptime.Notifications.Model.Abstractions;
using Uptime.Notifications.Model.Models;

namespace Uptime.Notifications.Logic.Services
{
    internal sealed class TemplateEngineProvider : ITemplateEngineProvider
    {
        private readonly IServiceProvider serviceProvider;

        public TemplateEngineProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public ITemplateEngine GetTemplateEngine(TemplateEngineType engine)
        {
            return engine switch
            {
                TemplateEngineType.Razor => serviceProvider.GetRequiredService<IRazorTemplateEngine>(),
                _ => throw new ArgumentException($"Unsupported template engine: {engine}", nameof(engine))
            };
        }
    }
}
