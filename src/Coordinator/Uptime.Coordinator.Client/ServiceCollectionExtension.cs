using Microsoft.Extensions.Options;
using Uptime.Coordinator.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCoordinatorClient(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpClient<IActivitiesClient, ActivitiesClient>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<CoordinatorClientSettings>>().Value;
                client.BaseAddress = settings.Url;
            });

            return serviceCollection;
        }
    }
}
