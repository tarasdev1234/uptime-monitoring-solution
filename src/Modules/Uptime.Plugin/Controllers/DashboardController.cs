using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Monitoring.Model.Models;
using Uptime.Monitoring.Data;
using Uptime.Mvc.Controllers;
using Uptime.Plugin.ViewModels.Dashboard;
using Microsoft.Extensions.Logging;
using Uptime.Monitoring.Model.Abstractions;
using Reliablesite.Authority.Authentication;
using System;
using Uptime.Plugin.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Reliablesite.Authority.Client;
using Reliablesite.Authority.Model;
using Uptime.Plugin.Services;

namespace Uptime.Plugin.Controllers
{
    [Authorize]
    public class DashboardController : BaseController {
        private readonly UptimeMonitoringContext monitoringContext;
        private readonly IEventsService eventSvc;
        private readonly IStatisticsService statisticsService;
      //  private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthoritySettings authSettings;
        private readonly ILogger logger;

        [TempData]
        public string StatusMessage { get; set; }

        public DashboardController (
            ApplicationDbContext ctx,
            UptimeMonitoringContext monitoringContext,
            ILogger<DashboardController> logger,
            IEventsService eventSvc,
            IStatisticsService statisticsService,
            IOptions<AuthoritySettings> authOptions
         //   UserManager<ApplicationUser> userManager
        ) : base(ctx) {
            this.monitoringContext = monitoringContext;
            this.logger = logger;
            this.eventSvc = eventSvc;
            this.statisticsService = statisticsService;
          //  _userManager = userManager;
            authSettings = authOptions.Value;
        }

        public async Task<IActionResult> Index 
            () {
            var userId = User.GetId();

            var mons = await monitoringContext.Monitors
                .Where(m => m.UserId == userId)
                .ToListAsync();

            var names = mons.ToDictionary(m => m.Id, m => m.Name);
            var monsForAvg = mons
                .Where(m => m.Status != MonitorStatus.Stopped && m.Status != MonitorStatus.Paused)
                .Select(m => m.Id)
                .ToArray();

            var active = mons.Count(m => m.Status != MonitorStatus.Stopped);
            var down = 0; // TODO
            var paused = mons.Count(m => m.Status == MonitorStatus.Paused);
            var up = mons.Count(m => m.Status == MonitorStatus.Started); // TODO, here was EventStatus.UP also
            var avg = await statisticsService.GetAverageUptimeAsync(userId);

            var latestDowntime = await eventSvc.GetLastDowntimeEventAsync(userId);
            var latestDowntimeMonitorName = default(string);

            if (latestDowntime != null)
            {
                names.TryGetValue(latestDowntime.MonitorId, out latestDowntimeMonitorName);
            }

            return View(new DashboardViewModel() {
                TotalMons = mons.Count,
                ActiveMons = active,
                PausedMons = paused,
                DownMons = down,
                UpMons = up,
                AvgUptime = avg,
                LatestDowntime = latestDowntime,
                LatestDowntimeMonitor = latestDowntimeMonitorName,
                MonitorTypes = new Dictionary<int, string> {
                    { (int)MonitorType.HTTP, "HTTP(S)" },
                    { (int)MonitorType.PING, "Ping" }
                }
            });
        }

        public async Task<IActionResult> Settings()
        {
            var userId = User.GetId();

            ApplicationUser user = null;

            try
            {
               //  user = await _userManager.GetUserAsync(HttpContext.User);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           
            var settings = await monitoringContext.UserSettings.FindAsync(userId);

            var model = new SettingsViewModel()
            {
                Email = User.GetEmail(),
                FullName = User.Identity.Name,
                InformFeatures = settings?.InformFeatures ?? false,
                InformDev = settings?.InformDev ?? false,
                TimeZones = new SelectList(TimeZoneInfo.GetSystemTimeZones(), "Id", "DisplayName"),
               // CurrentTimeZone = TimeZoneInfo.Local.DisplayName,
                CurrentTimeZone = settings?.CurrentTimeZone
             //   PhoneNumber = user != null ? user.PhoneNumber : ""
       
            };

            return View(model);
        }

        [HttpPost]
        [ActionName("Settings")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAccountSettings(UpdateSettingsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var userId = User.GetId();

            var settings = await monitoringContext.UserSettings.FindAsync(userId);

            if (settings == null)
            {
                settings = new UserSettings();
              //  settings.TimeZone = request.TimeZone;
                monitoringContext.Add(settings);
            }

            settings.InformDev = request.InformDev;
            settings.InformFeatures = request.InformFeatures;
            settings.CurrentTimeZone = request.CurrentTimeZone;
          //  settings.

            await monitoringContext.SaveChangesAsync();

            StatusMessage = "Your settings have been changed.";

            return RedirectToAction(nameof(Settings));

        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            await HttpContext.SignOutAsync();
            var externalLogoutUrl = QueryHelpers.AddQueryString(
                authSettings.LogoutPath,
                new Dictionary<string, string>{{ "returnUrl", returnUrl}, { "interactive", "false" } });
            return Redirect(externalLogoutUrl);
        }

        //public async Task <IActionResult> Support() 
        //{

        //    return View ();
        //}

        private void AddErrors (IdentityResult result) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
