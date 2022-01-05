using Microsoft.AspNetCore.Mvc.Rendering;

namespace Uptime.Plugin.ViewModels.Settings
{
    public class SettingsViewModel {
        public SelectList TimeZones { get; set; }

        public string CurrentTimeZone { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string CurrentPassword { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public bool InformFeatures { get; set; }

        public bool InformDev { get; set; }
    }
}
