using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Uptime.Data;
using Uptime.Mvc.Controllers;
using Uptime.Plugin.Services;
using Uptime.Plugin.ViewModels.Home;

namespace Uptime.Plugin.Controllers {
    public class HomeController : BaseController {
        private readonly IStatisticsService statisticsService;

        public HomeController(ApplicationDbContext ctx, IStatisticsService statisticsService) : base(ctx)
        {
            this.statisticsService = statisticsService;
        }

        public async Task<IActionResult> Index()
        {
            return View(new HomeViewModel
            {
                UsersCount = await statisticsService.GetUsersCountAsync(),
                MonitorsCount = await statisticsService.GetSitesMonitoredCountAsync(),
                NotificationsSent = await statisticsService.GetNotificationsCountAsync()
            });
        }

        public IActionResult Support() {
            return View();
        }

        public IActionResult About() {
            return View();
        }

        public IActionResult Faq() {
            return View();
        }
    }
}
