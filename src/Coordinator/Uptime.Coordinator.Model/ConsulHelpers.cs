using System;

namespace Uptime.Coordinator.Model
{
    public static class ConsulHelpers
    {
        public static Guid GetInstanceId(string serviceId)
        {
            var idx = serviceId.LastIndexOf('-');

            var rawId = idx > 0 ? serviceId.Substring(idx + 1, 32) : string.Empty;

            if (Guid.TryParse(rawId, out var result))
            {
                return result;
            }

            throw new ArgumentException($"Service id {serviceId} is not valid. Valid id format: {{service-name}}-{{guid}}. Guid must not contain hyphens.");
        }
    }
}
