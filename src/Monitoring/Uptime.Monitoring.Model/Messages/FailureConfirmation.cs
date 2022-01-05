using JsonSubTypes;
using Newtonsoft.Json;
using System;

namespace Uptime.Monitoring.Model.Messages
{
    public static class FailureConfirmationTypes
    {
        public const string Http = "Http";
        public const string Keyword = "Keyword";
        public const string Ping = "Ping";
    }

    [JsonConverter(typeof(JsonSubtypes), nameof(Type))]
    [JsonSubtypes.KnownSubType(typeof(KeywordFailureConfirmation), FailureConfirmationTypes.Keyword)]
    [JsonSubtypes.KnownSubType(typeof(HttpFailureConfirmation), FailureConfirmationTypes.Http)]
    [JsonSubtypes.KnownSubType(typeof(PingFailureConfirmation), FailureConfirmationTypes.Ping)]
    public class FailureConfirmation
    {
        public long MonitorId { get; set; }
        public long UserId { get; set; }
        public Guid OriginEventId { get; set; }
        public string Type { get; set; }
        public string Target { get; set; }
    }

    public class HttpFailureConfirmation : FailureConfirmation
    {
        public HttpFailureConfirmation()
        {
            Type = FailureConfirmationTypes.Http;
        }
    }

    public class PingFailureConfirmation : FailureConfirmation
    {
        public PingFailureConfirmation()
        {
            Type = FailureConfirmationTypes.Ping;
        }
    }

    public class KeywordFailureConfirmation : FailureConfirmation
    {
        public bool ShouldContain { get; set; }
        public string Keyword { get; set; }

        public KeywordFailureConfirmation()
        {
            Type = FailureConfirmationTypes.Keyword;
        }
    }
}
