using System;
using System.Collections.Generic;

namespace ModularVNext.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item)
        {
            foreach (var sourceItem in source)
            {
                yield return sourceItem;
            }
            yield return item;
        }
        public static IEnumerable<T> Concat<T>(this T source, IEnumerable<T> items)
        {
            yield return source;
            foreach (var item in items)
            {
                yield return item;
            }
        }
    }
}
