using Newtonsoft.Json;

namespace Uptime.Coordinator.Client
{
    public partial class ActivitiesClient
    {
        partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        }
    }
}
