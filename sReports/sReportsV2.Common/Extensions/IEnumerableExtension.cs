using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Common.Extensions
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<TSource> DistinctByExtension<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static string GetGraphDbQueryHeader(this IDictionary<string, string> namespaces)
        {
            return string.Join('\n', namespaces.Select(x => $"PREFIX {x.Key}: <{x.Value}>"));
        }
    }
}
