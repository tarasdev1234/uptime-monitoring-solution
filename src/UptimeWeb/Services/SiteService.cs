using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using OrchardCore.Environment.Cache;
using OrchardCore.Modules;
using OrchardCore.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uptime.Data.Models.Branding;
using Uptime.Mvc.Services;

namespace ModularTest.Services {
    public class SiteService : ISiteService {
        private readonly IMemoryCache _memoryCache;
        private readonly ISignal _signal;
        private readonly IServiceProvider _serviceProvider;
        private readonly IClock _clock;
        private const string SiteCacheKey = "SiteService";

        public SiteService (
            ISignal signal,
            IServiceProvider serviceProvider,
            IMemoryCache memoryCache,
            IClock clock) {
            _signal = signal;
            _serviceProvider = serviceProvider;
            _clock = clock;
            _memoryCache = memoryCache;
        }

        /// <inheritdoc/>
        public IChangeToken ChangeToken => _signal.GetToken(SiteCacheKey);

        /// <inheritdoc/>
        public async Task<ISite> GetSiteSettingsAsync () {
            ISite site;

            if (!_memoryCache.TryGetValue(SiteCacheKey, out site)) {
                //var session = GetSession();

                //site = await session.Query<SiteSettings>().FirstOrDefaultAsync();
                Brand brand = null;

                var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
                var ctx = httpContextAccessor.HttpContext.RequestServices.GetService<BrandContext>();

                brand = await ctx.GetCurrentBrandAsync();

                lock (_memoryCache) {
                    if (!_memoryCache.TryGetValue(SiteCacheKey, out site)) {
                        var properties = new Newtonsoft.Json.Linq.JObject();
                        properties["CurrentThemeName"] = brand?.Theme ?? "DefaultTheme";

                        site = new SiteSettings {
                            SiteSalt = Guid.NewGuid().ToString("N"),
                            SiteName = brand?.Name ?? "My Application",
                            PageSize = 10,
                            MaxPageSize = 100,
                            MaxPagedCount = 0,
                            TimeZoneId = _clock.GetSystemTimeZone().TimeZoneId,
                            Properties = properties
                        };
                        
                        _memoryCache.Set(SiteCacheKey, site);
                        _signal.SignalToken(SiteCacheKey);
                    }
                }
            }

            return site;
        }

        /// <inheritdoc/>
        public async Task UpdateSiteSettingsAsync (ISite site) {
            //var session = GetSession();

            //var existing = await session.Query<SiteSettings>().FirstOrDefaultAsync();

            //existing.BaseUrl = site.BaseUrl;
            //existing.Calendar = site.Calendar;
            //existing.Culture = site.Culture;
            //existing.HomeRoute = site.HomeRoute;
            //existing.MaxPagedCount = site.MaxPagedCount;
            //existing.MaxPageSize = site.MaxPageSize;
            //existing.PageSize = site.PageSize;
            //existing.Properties = site.Properties;
            //existing.ResourceDebugMode = site.ResourceDebugMode;
            //existing.SiteName = site.SiteName;
            //existing.SiteSalt = site.SiteSalt;
            //existing.SuperUser = site.SuperUser;
            //existing.TimeZoneId = site.TimeZoneId;
            //existing.UseCdn = site.UseCdn;

            //session.Save(existing);

            _memoryCache.Set(SiteCacheKey, site);
            _signal.SignalToken(SiteCacheKey);

            return;
        }

        //private YesSql.ISession GetSession () {
        //    var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
        //    return httpContextAccessor.HttpContext.RequestServices.GetService<YesSql.ISession>();
        //}
    }
}
