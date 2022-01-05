using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Uptime.Coordinator.Web.JsonConverters
{
    public class ConsulHealthStatusConverter : JsonConverter<Consul.HealthStatus>
    {
        public override Consul.HealthStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string str = reader.GetString();
            if (str == "passing")
                return Consul.HealthStatus.Passing;
            if (str == "warning")
                return Consul.HealthStatus.Warning;
            if (str == "critical")
                return Consul.HealthStatus.Critical;
            throw new ArgumentException("Invalid Check status value during deserialization");
        }

        public override void Write(Utf8JsonWriter writer, Consul.HealthStatus value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Status);
        }
    }
}
