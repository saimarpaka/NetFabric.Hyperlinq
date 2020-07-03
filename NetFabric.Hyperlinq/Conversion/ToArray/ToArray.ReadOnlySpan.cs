using System;

namespace NetFabric.Hyperlinq
{
    public static partial class ArrayExtensions
    {
        
        static TSource[] ToArray<TSource>(this ReadOnlySpan<TSource> source, Predicate<TSource> predicate)
        {
            var builder = new LargeArrayBuilder<TSource>(initialize: true);
            for (var index = 0; index < source.Length; index++)
            {
                if (predicate(source[index]))
                    builder.Add(source[index]);
            }
            return builder.ToArray();
        }

        
        static TSource[] ToArray<TSource>(this ReadOnlySpan<TSource> source, PredicateAt<TSource> predicate)
        {
            var builder = new LargeArrayBuilder<TSource>(initialize: true);
            for (var index = 0; index < source.Length; index++)
            {
                if (predicate(source[index], index))
                    builder.Add(source[index]);
            }
            return builder.ToArray();
        }

        
        static TResult[] ToArray<TSource, TResult>(this ReadOnlySpan<TSource> source, NullableSelector<TSource, TResult> selector)
        {
            var array = new TResult[source.Length];
            ArrayExtensions.Copy(source, array, selector);
            return array;
        }

        
        static TResult[] ToArray<TSource, TResult>(this ReadOnlySpan<TSource> source, NullableSelectorAt<TSource, TResult> selector)
        {
            var array = new TResult[source.Length];
            ArrayExtensions.Copy(source, array, selector);
            return array;
        }

        
        static TResult[] ToArray<TSource, TResult>(this ReadOnlySpan<TSource> source, Predicate<TSource> predicate, NullableSelector<TSource, TResult> selector)
        {
            var builder = new LargeArrayBuilder<TResult>(initialize: true);
            for (var index = 0; index < source.Length; index++)
            {
                if (predicate(source[index]))
                    builder.Add(selector(source[index]));
            }
            return builder.ToArray();
        }
    }
}

