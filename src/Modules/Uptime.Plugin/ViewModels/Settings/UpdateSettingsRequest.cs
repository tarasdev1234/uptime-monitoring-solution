namespace Uptime.Plugin.ViewModels.Settings
{
    public sealed class UpdateSettingsRequest
    {
        public bool InformFeatures { get; set; }

        public bool InformDev { get; set; }

        public string CurrentTimeZone { get; set; }
    }
}
