using System;
using System.Collections.Generic;
using System.Text;

namespace Uptime.Plugin.ViewModels.Dashboard {
    public class CreateAlertViewModel {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public int NotificationType { get; set; }

        public int Type { get; set; }

        public bool Active { get; set; }
    }
}
