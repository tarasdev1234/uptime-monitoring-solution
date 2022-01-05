using System.Threading.Tasks;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Plugin.Services
{
    public interface IStatisticsService
    {
        Task<int> GetUsersCountAsync();
        Task<int> GetSitesMonitoredCountAsync();
        Task<int> GetNotificationsCountAsync();
        Task<AvgUptime> GetAverageUptimeAsync(long userId);
    }
}
