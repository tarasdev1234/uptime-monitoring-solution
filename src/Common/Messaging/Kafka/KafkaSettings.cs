using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Messaging.Kafka
{
    public sealed class KafkaSettings
    {
        [Required(AllowEmptyStrings = false)]
        public string Hosts { get; set; } = string.Empty;
        public string? ConsumerGroup { get; set; }
        public IReadOnlyCollection<TopicSettings> Topics { get; set; } = Array.Empty<TopicSettings>();
    }

    public sealed class TopicSettings
    {
        [Required(AllowEmptyStrings = false)]
        public string Topic { get; set; } = string.Empty;
        public string? Alias { get; set; }
        public string? ConsumerGroup { get; set; }
    }
}
