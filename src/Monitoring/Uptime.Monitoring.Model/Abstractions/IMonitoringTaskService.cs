using System.Collections.Generic;
using System.Threading.Tasks;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Model.Abstractions
{
    public interface IMonitoringTaskService
    {
        void StartOneTimeTask(MonitorTask monitor);

        Task StartAsync(long taskId);

        Task StopAsync(long taskId);

        Task PauseAsync(long taskId);

        bool IsRunning(long taskId);

        HashSet<long> GetRunningIds();
    }
}
