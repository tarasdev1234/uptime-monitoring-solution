using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Reliablesite.SqlXmlRepository
{
    public class SqlXmlRepository : IXmlRepository
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<SqlXmlRepository> logger;

        public SqlXmlRepository(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            logger = serviceProvider.GetRequiredService<ILogger<SqlXmlRepository>>();
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<XmlRepositoryDbContext>();
                var entries = ctx.DataProtectionKeys.ToList();
                logger.LogDebug($"Obtained {entries.Count} keys from database: {FormatKeysForLog(entries)}");
                return entries.Select(x => x.Value).ToList();
            }
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            logger.LogDebug($"Saving protection key {friendlyName} to database");
            using (var scope = serviceProvider.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<XmlRepositoryDbContext>();

                ctx.DataProtectionKeys.Add(new XmlRepositoryEntry
                {
                    Value = element,
                    FriendlyName = friendlyName
                });

                ctx.SaveChanges();
            }
        }

        private static string FormatKeysForLog(IEnumerable<XmlRepositoryEntry> keys)
        {
            return string.Join(", ", keys.Select(x => x.FriendlyName));
        }
    }
}
