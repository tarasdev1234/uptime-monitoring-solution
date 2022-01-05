using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Reliablesite.Service.Core.StartupFilters
{
    internal class DefaultConfigurationFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseRequestLogging();
                next(app);
                app.UseDefaultServices();
            };
        }
    }
}
