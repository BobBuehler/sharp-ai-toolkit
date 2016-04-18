using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

public static class Extensions
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var s in source)
        {
            action(s);
        }
    }
    
    public static T MinByValue<T, K>(this IEnumerable<T> source, Func<T, K> selector)
    {
        var comparer = Comparer<T>.Default;
        
        var enumerator = source.GetEnumerator();
        enumerator.MoveNext();
        
        var min = enumerator.Current;
        var minV = selector(min);
        
        while (enumerator.MoveNext())
        {
            var s = enumerator.Current;
            var v = selector(s);
            if (comparer.Compare(v, minV) < 0)
            {
                min = s;
                minV = v;
            }
        }
        return min;
    }
    
    public static T MaxByValue<T, K>(this IEnumerable<T> source, Func<T, K> selector)
    {
        var comparer = Comparer<T>.Default;
        
        var enumerator = source.GetEnumerator();
        enumerator.MoveNext();
        
        var max = enumerator.Current;
        var maxV = selector(max);
        
        while (enumerator.MoveNext())
        {
            var s = enumerator.Current;
            var v = selector(s);
            if (comparer.Compare(v, maxV) > 0)
            {
                max = s;
                maxV = v;
            }
        }
        return max;
    }

    public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> func, Dictionary<T, TResult> cache = null)
    {
        cache = cache ?? new Dictionary<T, TResult>();
        return t =>
        {
            TResult result;
            if (!cache.TryGetValue(t, out result))
            {
                result = func(t);
                cache[t] = result;
            }
            return result;
        }
    }
}