using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reliablesite.Service.Core.StartupFilters;

namespace Reliablesite.Service.Core.Extensions
{
    public static class OptionsBuilderValidationExtensions
    {
        public static OptionsBuilder<TOptions> ValidateEagerly<TOptions>(this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class, new()
        {
            optionsBuilder.Services.AddTransient<IStartupFilter, StartupOptionsValidation<TOptions>>();
            return optionsBuilder;
        }
    }
}
