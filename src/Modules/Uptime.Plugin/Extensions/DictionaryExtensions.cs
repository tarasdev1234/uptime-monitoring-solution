using System.Collections.Generic;

namespace Uptime.Plugin.Extensions
{
    internal static class DictionaryExtensions
    {
        public static TValue TryGetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default)
        {
            return dict.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}
