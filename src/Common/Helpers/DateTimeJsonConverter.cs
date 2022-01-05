using Newtonsoft.Json.Converters;

namespace Helpers {
    public sealed class DateTimeJsonConverter : IsoDateTimeConverter {
        public DateTimeJsonConverter () {
            DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }
    }
}