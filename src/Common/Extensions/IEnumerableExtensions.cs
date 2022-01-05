using System;
using System.Collections.Generic;

namespace Uptime.Extensions {
    public static class IEnumerableExtensions {
        public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> data, Func<T, bool> predicate) {
            foreach (var item in data) {
                yield return item;

                if (!predicate(item)) {
                    break;
                }
            }
        }
    }
}
