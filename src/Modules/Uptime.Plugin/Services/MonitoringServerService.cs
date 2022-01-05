using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Uptime.Plugin.Services {
    public class MonitoringServerService : IMonitoringServerService {
        public static readonly string API_KEY_HEADER = "X-SERVER-API-KEY";

        private readonly HttpClient httpClient;
        private readonly ILogger<MonitoringServerService> logger;

        public MonitoringServerService (HttpClient httpClient, ILogger<MonitoringServerService> logger) {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        private HttpClient GetHttpClient (string key) {
            httpClient.DefaultRequestHeaders.Remove(API_KEY_HEADER);
            httpClient.DefaultRequestHeaders.Add(API_KEY_HEADER, key);

            return httpClient;
        }

        public async Task StartTask (string url, string key, long id) {
            var uri = $"{url}api/tasks/{id}/start";
            var content = new StringContent("", Encoding.UTF8, "application/json");

            var response = await GetHttpClient(key).PostAsync(uri, content);

            response.EnsureSuccessStatusCode();
        }

        public async Task StopTask (string url, string key, long id) {
            var uri = $"{url}api/tasks/{id}/stop";
            var response = await GetHttpClient(key).DeleteAsync(uri);

            response.EnsureSuccessStatusCode();
        }

        public async Task PauseTask (string url, string key, long id) {
            var uri = $"{url}api/tasks/{id}/pause";
            var content = new StringContent("", Encoding.UTF8, "application/json");

            var response = await GetHttpClient(key).PostAsync(uri, content);

            response.EnsureSuccessStatusCode();
        }
    }
}
