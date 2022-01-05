using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Uptime.Plugin.Services {
    public interface IMonitoringServerService {
        Task StartTask (string url, string key, long id);

        Task StopTask (string url, string key, long id);

        Task PauseTask (string url, string key, long id);
    }
}
