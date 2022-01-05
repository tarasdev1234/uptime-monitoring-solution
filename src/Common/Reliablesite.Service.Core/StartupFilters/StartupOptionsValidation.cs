using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Reliablesite.Service.Core.StartupFilters
{
    public class StartupOptionsValidation<T> : IStartupFilter where T : class, new()
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                var options = builder.ApplicationServices.GetService<IOptions<T>>();
                if (options != null)
                {
                    // Retrieve the value to trigger validation
                    var optionsValue = ((IOptions<object>)options).Value;
                }

                next(builder);
            };
        }
    }
}
