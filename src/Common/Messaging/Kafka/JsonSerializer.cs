using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Messaging.Kafka
{
    internal sealed class JsonSerializer<T> : ISerializer<T>, IDeserializer<T>
    {
        private readonly JsonSerializerSettings? settings;

        public JsonSerializer()
        {
        }

        public JsonSerializer(JsonSerializerSettings? settings)
        {
            this.settings = settings;
        }
        
        [return: MaybeNull]
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull)
            {
                return default!;
            }

            return JsonConvert.DeserializeObject<T>(
                Encoding.UTF8.GetString(data),
                settings);
        }

        public byte[]? Serialize(T data, SerializationContext context)
        {
            if (data == null)
            {
                return null;
            }

            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
        }
    }
}
