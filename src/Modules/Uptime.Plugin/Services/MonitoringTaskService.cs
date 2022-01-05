using Messaging;
using System.Threading.Tasks;
using Uptime.Monitoring.Model.Messages;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Plugin.Services
{
    public sealed class MonitoringTaskService : IMonitoringTaskService
    {
        private readonly IProducer<SetMonitorStatus> producer;

        public MonitoringTaskService(IProducer<SetMonitorStatus> producer)
        {
            this.producer = producer;
        }

        public Task StartAsync(long monitorId)
        {
            return producer.SendAsync(monitorId.ToString(), new SetMonitorStatus
            {
                MonitorId = monitorId,
                Status = MonitorStatus.Started
            });
        }

        public Task StopAsync(long monitorId)
        {
            return producer.SendAsync(monitorId.ToString(), new SetMonitorStatus
            {
                MonitorId = monitorId,
                Status = MonitorStatus.Stopped
            });
        }
    }
}
