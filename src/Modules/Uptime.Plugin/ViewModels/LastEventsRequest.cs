using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Uptime.Plugin.ViewModels {
    public class LastEventsRequest {
        public int PageSize { get; set; }

        public long MonitorId { get; set; }

        public string PagingState { get; set; }

        public string StartingId { get; set; }

        [JsonIgnore]
        public byte[] paging { get; set; }
    }
}
