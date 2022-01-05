using Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Uptime.Monitoring.Model.Models {
    public abstract class Monitor
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public MonitorStatus Status { get; set; }

        public int Interval { get; set; }

        /// <summary>
        /// Repeat count.
        /// <1 - repeat infinite times
        /// 1 - no repeat, just fire once
        /// >0 - repeat N times
        /// </summary>
        public int Repeat { get; set; }
        
        public MonitorType Type { get; private set; }

        public int AuthType { get; set; }

        public Guid LastExecutor { get; set; }

        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTime CreatedDate { get; set; }

        [JsonIgnore]
        public List<MonitorAlertContact> AlertContacts { get; set; }

        public Monitor(MonitorType type)
        {
            Type = type;
        }
    }
}
