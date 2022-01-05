using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reliablesite.SqlXmlRepository;

namespace Microsoft.AspNetCore.DataProtection
{
    public static class DataProtectionBuilderExtensions
    {
        public static IDataProtectionBuilder PersistKeysToSqlStorage(this IDataProtectionBuilder dataProtectionBuilder, string connectionString)
        {
            dataProtectionBuilder
                .Services
                .AddDbContext<XmlRepositoryDbContext>(
                    options => options.UseSqlServer(
                        connectionString,
                        sqlOptions => sqlOptions.MigrationsHistoryTable("__DataProtectionKeysMigrationHistory")))
                .AddSingleton<IConfigureOptions<KeyManagementOptions>>(sp =>
                {
                    return new ConfigureOptions<KeyManagementOptions>(options =>
                    {
                        options.XmlRepository = new SqlXmlRepository(sp);
                    });
                });

            return dataProtectionBuilder;
        }
    }
}
