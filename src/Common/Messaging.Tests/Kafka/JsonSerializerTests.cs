using Confluent.Kafka;
using Messaging.Kafka;
using NUnit.Framework;
using System;
using System.Dynamic;
using System.Text;

namespace Messaging.Tests.Kafka
{
    public class JsonSerializerTests
    {
        internal class Foo
        {
            public string Value { get; private set; }

            public Foo(string value)
            {
                Value = value;
            }

            private Foo()
            {
            }
        }

        [Test]
        public void Should_serialize_and_deserialize_expandoobject()
        {
            var serializer = new JsonSerializer<ExpandoObject>();

            var date = new DateTime(2001, 01, 02, 14, 12, 15);
            dynamic data = new ExpandoObject();
            data.String = "string";
            data.Date = date;
            data.Integer = 15;
            data.Object = new
            {
                SubString = "substring",
                SubDate = date
            };

            var serialized = Encoding.UTF8.GetString(serializer.Serialize(data, SerializationContext.Empty));

            var deserialized = serializer.Deserialize(Encoding.UTF8.GetBytes(serialized), false, SerializationContext.Empty);

            Assert.AreEqual(data.String, deserialized.String);
            Assert.AreEqual(data.Date, deserialized.Date);
            Assert.AreEqual(data.Integer, deserialized.Integer);
            Assert.AreEqual(data.Object.SubString, deserialized.Object.SubString);
            Assert.AreEqual(data.Object.SubDate, deserialized.Object.SubDate);
        }

        [Test]
        public void Should_deserialize_object_with_private_ctor_and_property()
        {
            var serializer = new JsonSerializer<Foo>();
            var serialized = "{'Value': 'some_value_here'}";

            var deserialized = serializer.Deserialize(Encoding.UTF8.GetBytes(serialized), false, SerializationContext.Empty);

            Assert.AreEqual("some_value_here", deserialized.Value);
        }
    }
}
