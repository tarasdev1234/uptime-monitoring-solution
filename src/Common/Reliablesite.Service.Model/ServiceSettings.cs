using System;

namespace Reliablesite.Service.Model
{
    public sealed class ServiceSettings
    {
        [RequireNonDefault]
        public Guid InstanceId { get; set; }
        public string Name { get; set; }
    }
}
