using Microsoft.Extensions.Options;
using Reliablesite.Authority.Client;
using Reliablesite.Authority.Model;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddAuthorityClient(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddHttpClient<IUsersClient, UsersClient>((sp, client) =>
                {
                    var settings = sp.GetRequiredService<IOptions<AuthoritySettings>>().Value;
                    client.BaseAddress = settings.Url;
                })
                .AddUserAccessTokenHandler();

            serviceCollection
                .AddHttpClient<IRolesClient, RolesClient>((sp, client) =>
                {
                    var settings = sp.GetRequiredService<IOptions<AuthoritySettings>>().Value;
                    client.BaseAddress = settings.Url;
                })
                .AddUserAccessTokenHandler();

            serviceCollection
                .AddHttpClient<IUsersInternalClient, UsersInternalClient>((sp, client) =>
                {
                    var settings = sp.GetRequiredService<IOptions<AuthoritySettings>>().Value;
                    client.BaseAddress = settings.Url;
                })
                .AddUserAccessTokenHandler();

            return serviceCollection;
        }
    }
}
