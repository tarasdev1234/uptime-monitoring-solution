using AutoMapper;
using Uptime.Notifications.Web;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddTransient<Profile, DtoMapperProfile>();
        }
    }
}
