using System.Threading.Tasks;

namespace Uptime.Plugin.Services
{
    public interface IMonitoringTaskService
    {
        Task StartAsync(long monitorId);

        Task StopAsync(long monitorId);
    }
}
