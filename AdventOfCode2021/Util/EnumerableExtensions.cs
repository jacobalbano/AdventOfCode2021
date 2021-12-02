using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class EnumerableExtensions
{
    public static IEnumerable<IEnumerable<T>> PartitionBy<T>(this IEnumerable<T> self, Func<T, bool> delimit)
    {
        using var e = self.GetEnumerator();
        while (e.MoveNext())
            yield return Inner();

        IEnumerable<T> Inner()
        {
            do
            {
                if (delimit(e.Current))
                    yield break;

                yield return e.Current;
            } while (e.MoveNext());
        }
    }

    public static IEnumerable<(T, T)> Lag<T>(this IEnumerable<T> self)
    {
        using var e = self.GetEnumerator();
        if (!e.MoveNext())
            yield break;

        T first = e.Current;
        while (e.MoveNext())
            yield return (first, first = e.Current);
    }

    public static IEnumerable<T[]> ChunkBy<T>(this IEnumerable<T> self, int chunkSize)
    {
        using var e = self.GetEnumerator();
        for (bool go = e.MoveNext(); go;)
        {
            var result = new T[chunkSize];
            for (int i = 0; i < chunkSize; i++)
            {
                result[i] = e.Current;
                if (!(go = e.MoveNext()))
                    break;
            }

            yield return result;
        }
    }

    public static IEnumerable<T[]> WindowBy<T>(this IEnumerable<T> self, int windowSize)
    {
        using var e = self.GetEnumerator();
        bool go = e.MoveNext();
        if (!go) yield break;
        
        //  make first window
        var result = new T[windowSize];
        for (int i = 0; i < windowSize; i++)
        {
            result[i] = e.Current;
            if (!(go = e.MoveNext()))
                break;
        }

        yield return result;

        //  slide it
        while (go)
        {
            var last = result;
            result = new T[windowSize];
            for (int i = 1; i < windowSize; i++)
                result[i - 1] = last[i];

            result[windowSize - 1] = e.Current;
            yield return result;
            go = e.MoveNext();
        }
    }

    public static IEnumerable<T> As<T>(this IEnumerable untyped)
    {
        foreach (var x in untyped)
            yield return (T)x;
    }

    public delegate (bool, TOut) SelectWhereSelector<TIn, TOut>(TIn input);

    public static IEnumerable<TOut> SelectWhere<TIn, TOut>(this IEnumerable<TIn> self, SelectWhereSelector<TIn, TOut> selector)
    {
        foreach (var x in self)
        {
            var (success, result) = selector(x);
            if (success) yield return result;
        }
    }

    public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
    {
        IEnumerable<IEnumerable<T>> result = new[] { Enumerable.Empty<T>() };
        foreach (var sequence in sequences)
        {
            var localSequence = sequence;
            result = result.SelectMany(
              _ => localSequence,
              (seq, item) => seq.Concat(new[] { item })
            );
        }
        return result;
    }
}
