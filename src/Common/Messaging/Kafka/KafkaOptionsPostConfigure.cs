using Microsoft.Extensions.Options;
using Reliablesite.Service.Model;
using System;

namespace Messaging.Kafka
{
    internal sealed class KafkaOptionsPostConfigure : IPostConfigureOptions<KafkaSettings>
    {
        private readonly ServiceSettings serviceSettings;

        public KafkaOptionsPostConfigure(IOptions<ServiceSettings> serviceOptions)
        {
            serviceSettings = serviceOptions.Value;
        }

        public void PostConfigure(string name, KafkaSettings options)
        {
            if (string.IsNullOrEmpty(options.ConsumerGroup))
            {
                options.ConsumerGroup = serviceSettings.Name + "-group";
            }
        }
    }
}
