using System;
using System.Collections;
using System.Collections.Generic;

namespace NetFabric.Hyperlinq
{
    public static class EnumerableBindings
    {
        public static int Count<TSource>(this IEnumerable<TSource> source)
            => Enumerable.Count<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source);

        public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => Enumerable.Count<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source, predicate);

        public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => Enumerable.All<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source, predicate);

        public static bool Any<TSource>(this IEnumerable<TSource> source)
            => Enumerable.Any<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source);

        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => Enumerable.Any<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source, predicate);

        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
            => source.Contains(value);

        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
            => Enumerable.Contains<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source, value, comparer);

        public static Enumerable.SelectEnumerable<IEnumerable<TSource>, IEnumerator<TSource>, TSource, TResult> Select<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> selector) 
            => Enumerable.Select<IEnumerable<TSource>, IEnumerator<TSource>, TSource, TResult>(source, selector);

        public static Enumerable.SelectManyEnumerable<IEnumerable<TSource>, IEnumerator<TSource>, TSource, TSubEnumerable, TSubEnumerator, TResult> SelectMany<TSource, TSubEnumerable, TSubEnumerator, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TSubEnumerable> selector) 
            where TSubEnumerable : IValueEnumerable<TResult, TSubEnumerator>
            where TSubEnumerator : struct, IValueEnumerator<TResult>
            => Enumerable.SelectMany<IEnumerable<TSource>, IEnumerator<TSource>, TSource, TSubEnumerable, TSubEnumerator, TResult>(source, selector);

        public static Enumerable.WhereEnumerable<IEnumerable<TSource>, IEnumerator<TSource>, TSource> Where<TSource>(
            this IEnumerable<TSource> source, 
            Func<TSource, bool> predicate) 
            => Enumerable.Where<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source, predicate);

        public static TSource First<TSource>(this IEnumerable<TSource> source)
            => Enumerable.First<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source);

        public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => Enumerable.First<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source, predicate);

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
            => Enumerable.FirstOrDefault<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source);

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => Enumerable.FirstOrDefault<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source, predicate);

        public static TSource? FirstOrNull<TSource>(this IEnumerable<TSource> source)
            where TSource : struct
            => Enumerable.FirstOrNull<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source);

        public static TSource? FirstOrNull<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            where TSource : struct
            => Enumerable.FirstOrNull<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source, predicate);

        public static TSource Single<TSource>(this IEnumerable<TSource> source)
            => Enumerable.Single<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source);

        public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => Enumerable.Single<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source, predicate);

        public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source)
            => Enumerable.SingleOrDefault<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source);

        public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => Enumerable.SingleOrDefault<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source, predicate);

        public static TSource? SingleOrNull<TSource>(this IEnumerable<TSource> source)
            where TSource : struct
            => Enumerable.SingleOrNull<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source);

        public static TSource? SingleOrNull<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            where TSource : struct
            => Enumerable.SingleOrNull<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source, predicate);

        public static IEnumerable<TSource> AsEnumerable<TSource>(this IEnumerable<TSource> source)
            => source;

        public static Enumerable.AsValueEnumerableEnumerable<IEnumerable<TSource>, IEnumerator<TSource>, TSource> AsValueEnumerable<TSource>(this IEnumerable<TSource> source)
            => Enumerable.AsValueEnumerable<IEnumerable<TSource>, IEnumerator<TSource>,  TSource>(source);

        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
            => Enumerable.ToArray<IEnumerable<TSource>, IEnumerator<TSource>, TSource>(source);

        public static IEnumerable<TSource> ToList<TSource>(this IEnumerable<TSource> source)
            => source;
    }
}