using System;
using System.Collections.Generic;

namespace ModularVNext.Extensions
{
    public static class EnumerableExtensions
    {
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
