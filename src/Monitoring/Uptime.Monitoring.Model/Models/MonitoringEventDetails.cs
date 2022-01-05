using System;
using System.Collections.Generic;

namespace Uptime.Monitoring.Model.Models
{
    public sealed class MonitoringEventDetails : Dictionary<string, string>
    {
        public static class Names
        {
            public const string Description = "Description";
            public const string Exception = "Exception";
            public const string Host = "Host";
            public const string ResponseTime = "ResponseTime";
        }

        public MonitoringEventDetails()
        {
        }

        public MonitoringEventDetails(MonitoringEventDetails details) : base(details)
        {
        }

        public MonitoringEventDetails(IDictionary<string, string> dictionary) : base(dictionary)
        {
        }

        public string Description => SafeGet(Names.Description);

        private string SafeGet(string key)
        {
            return TryGetValue(key, out var value) ? value : null;
        }
    }
}
