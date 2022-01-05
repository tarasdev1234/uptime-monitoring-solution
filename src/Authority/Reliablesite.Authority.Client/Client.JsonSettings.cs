using Newtonsoft.Json;

namespace Reliablesite.Authority.Client
{
    public partial class UsersClient
    {
        partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        }
    }
}
