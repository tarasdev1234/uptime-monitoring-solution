using Cassandra.Mapping.TypeConversion;
using System;
using System.Collections.Generic;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Cassandra
{
    internal sealed class CustomTypeConverter : TypeConverter
    {
        protected override Func<TDatabase, TPoco> GetUserDefinedFromDbConverter<TDatabase, TPoco>()
        {
            var tuple = new Tuple<TDatabase, TPoco>(default, default);

            return tuple switch
            {
                Tuple<long, TimeSpan> _ => (TDatabase value)
                    => (TPoco)(object)LongToTimeSpan((long)(object)value),
                Tuple<IDictionary<string, string>, MonitoringEventDetails> _
                    => (TDatabase value) => (TPoco)(object)DictionaryToMonitoringEventDetails((IDictionary<string, string>)(object)value),
                _ => null,
            };
        }

        protected override Func<TPoco, TDatabase> GetUserDefinedToDbConverter<TPoco, TDatabase>()
        {
            var tuple = new Tuple<TPoco, TDatabase>(default, default);

            return tuple switch
            {
                Tuple<TimeSpan, long> _ => (TPoco value) => (TDatabase)(object)TimeSpanToLong((TimeSpan)(object)value),
                _ => null,
            };
        }

        private long TimeSpanToLong(TimeSpan value) => (long)value.TotalMilliseconds;

        private TimeSpan LongToTimeSpan(long value) => TimeSpan.FromMilliseconds(value);

        private MonitoringEventDetails DictionaryToMonitoringEventDetails(IDictionary<string, string> dictionary)
            => dictionary != null ? new MonitoringEventDetails(dictionary) : null;
    }
}
