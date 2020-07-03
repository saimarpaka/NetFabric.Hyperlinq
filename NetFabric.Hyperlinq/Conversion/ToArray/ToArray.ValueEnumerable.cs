using System;
using System.Collections.Generic;

namespace NetFabric.Hyperlinq
{
    public static partial class ValueEnumerableExtensions
    {
        
        public static TSource[] ToArray<TEnumerable, TEnumerator, TSource>(this TEnumerable source)
            where TEnumerable : IValueEnumerable<TSource, TEnumerator>
            where TEnumerator : struct, IEnumerator<TSource>
        {
            switch (source)
            {
                case ICollection<TSource> collection:
                    var count = collection.Count;
                    if (count == 0)
                        return Array.Empty<TSource>();

                    var buffer = new TSource[count];
                    collection.CopyTo(buffer, 0);
                    return buffer;

                default:
                    {
                        using var builder = new LargeArrayBuilder<TSource>(initialize: true);
                        using var enumerator = source.GetEnumerator();
                        while (enumerator.MoveNext())
                            builder.Add(enumerator.Current);
                        return builder.ToArray();
                    }
            }
        }

        
        static TSource[] ToArray<TEnumerable, TEnumerator, TSource>(this TEnumerable source, Predicate<TSource> predicate)
            where TEnumerable : IValueEnumerable<TSource, TEnumerator>
            where TEnumerator : struct, IEnumerator<TSource>
        {
            using var builder = new LargeArrayBuilder<TSource>(initialize: true);
            using var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (predicate(item))
                    builder.Add(item);
            }
            return builder.ToArray();
        }

        
        static TSource[] ToArray<TEnumerable, TEnumerator, TSource>(this TEnumerable source, PredicateAt<TSource> predicate)
            where TEnumerable : IValueEnumerable<TSource, TEnumerator>
            where TEnumerator : struct, IEnumerator<TSource>
        {
            using var builder = new LargeArrayBuilder<TSource>(initialize: true);
            using var enumerator = source.GetEnumerator();
            checked
            {
                for (var index = 0; enumerator.MoveNext(); index++)
                {
                    var item = enumerator.Current;
                    if (predicate(item, index))
                        builder.Add(item);
                }
            }
            return builder.ToArray();
        }

        
        static TResult[] ToArray<TEnumerable, TEnumerator, TSource, TResult>(this TEnumerable source, NullableSelector<TSource, TResult> selector)
            where TEnumerable : IValueEnumerable<TSource, TEnumerator>
            where TEnumerator : struct, IEnumerator<TSource>
        {
            using var builder = new LargeArrayBuilder<TResult>(initialize: true);
            using var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
                builder.Add(selector(enumerator.Current));
            return builder.ToArray();
        }

        
        static TResult[] ToArray<TEnumerable, TEnumerator, TSource, TResult>(this TEnumerable source, NullableSelectorAt<TSource, TResult> selector)
            where TEnumerable : IValueEnumerable<TSource, TEnumerator>
            where TEnumerator : struct, IEnumerator<TSource>
        {
            using var builder = new LargeArrayBuilder<TResult>(initialize: true);
            using var enumerator = source.GetEnumerator();
            checked
            {
                for (var index = 0; enumerator.MoveNext(); index++)
                    builder.Add(selector(enumerator.Current, index));
            }
            return builder.ToArray();
        }

        
        static TResult[] ToArray<TEnumerable, TEnumerator, TSource, TResult>(this TEnumerable source, Predicate<TSource> predicate, NullableSelector<TSource, TResult> selector)
            where TEnumerable : IValueEnumerable<TSource, TEnumerator>
            where TEnumerator : struct, IEnumerator<TSource>
        {
            using var builder = new LargeArrayBuilder<TResult>(initialize: true);
            using var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (predicate(item))
                    builder.Add(selector(item));
            }
            return builder.ToArray();
        }
    }
}