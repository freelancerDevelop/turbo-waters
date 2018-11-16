using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> target)
    {
        Random r = new Random();

        return target.OrderBy(x => (r.Next()));
    }
}
