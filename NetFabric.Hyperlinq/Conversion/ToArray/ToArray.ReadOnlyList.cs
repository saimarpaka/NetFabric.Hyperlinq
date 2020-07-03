using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NetFabric.Hyperlinq
{
    public static partial class ReadOnlyListExtensions
    {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSource[] ToArray<TList, TSource>(this TList source)
            where TList : IReadOnlyList<TSource>
            => ToArray<TList, TSource>(source, 0, source.Count);

        static TSource[] ToArray<TList, TSource>(this TList source, int skipCount, int takeCount)
            where TList : IReadOnlyList<TSource>
        {
            var array = new TSource[takeCount];
            ReadOnlyListExtensions.Copy(source, skipCount, array, 0, takeCount);
            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<TSource> ToArray<TList, TSource>(this TList source, ArrayPool<TSource> pool)
            where TList : IReadOnlyList<TSource>
            => ToArray(source, 0, source.Count, pool);

        static ArraySegment<TSource> ToArray<TList, TSource>(this TList source, int skipCount, int takeCount, ArrayPool<TSource> pool)
            where TList : IReadOnlyList<TSource>
        {
            var result = new ArraySegment<TSource>(pool.Rent(takeCount), 0, takeCount);
            ReadOnlyListExtensions.Copy(source, skipCount, result.Array, 0, takeCount);
            return result;
        }

#if SPAN_SUPPORTED

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IMemoryOwner<TSource> ToArray<TList, TSource>(this TList source, MemoryPool<TSource> pool)
            where TList : IReadOnlyList<TSource>
            => ToArray(source, 0, source.Count, pool);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IMemoryOwner<TSource> ToArray<TList, TSource>(this TList source, int skipCount, int takeCount, MemoryPool<TSource> pool)
            where TList : IReadOnlyList<TSource>
        {
            var result = pool.RentSliced(takeCount);
            ReadOnlyListExtensions.Copy<TList, TSource>(source, skipCount, result.Memory.Span, takeCount);
            return result;
        }

#endif

        static TSource[] ToArray<TList, TSource>(this TList source, Predicate<TSource> predicate, int skipCount, int takeCount)
            where TList : IReadOnlyList<TSource>
        {
            using var builder = new LargeArrayBuilder<TSource>(initialize: true);
            var end = skipCount + takeCount;
            for (var index = skipCount; index < end; index++)
            {
                if (predicate(source[index]))
                    builder.Add(source[index]);
            }
            return builder.ToArray();
        }


        static TSource[] ToArray<TList, TSource>(this TList source, PredicateAt<TSource> predicate, int skipCount, int takeCount)
            where TList : IReadOnlyList<TSource>
        {
            using var builder = new LargeArrayBuilder<TSource>(initialize: true);
            if (skipCount == 0)
            {
                for (var index = 0; index < takeCount; index++)
                {
                    if (predicate(source[index], index))
                        builder.Add(source[index]);
                }
            }
            else
            {
                for (var index = 0; index < takeCount; index++)
                {
                    if (predicate(source[index + skipCount], index))
                        builder.Add(source[index + skipCount]);
                }
            }
            return builder.ToArray();
        }


        static TResult[] ToArray<TList, TSource, TResult>(this TList source, NullableSelector<TSource, TResult> selector, int skipCount, int takeCount)
            where TList : IReadOnlyList<TSource>
        {
            var array = new TResult[takeCount];
            ReadOnlyListExtensions.Copy(source, skipCount, array, 0, takeCount, selector);
            return array;
        }


        static TResult[] ToArray<TList, TSource, TResult>(this TList source, NullableSelectorAt<TSource, TResult> selector, int skipCount, int takeCount)
            where TList : IReadOnlyList<TSource>
        {
            var array = new TResult[takeCount];
            ReadOnlyListExtensions.Copy(source, skipCount, array, 0, takeCount, selector);
            return array;
        }


        static TResult[] ToArray<TList, TSource, TResult>(this TList source, Predicate<TSource> predicate, NullableSelector<TSource, TResult> selector, int skipCount, int takeCount)
            where TList : IReadOnlyList<TSource>
        {
            using var builder = new LargeArrayBuilder<TResult>(initialize: true);
            var end = skipCount + takeCount;
            for (var index = skipCount; index < end; index++)
            {
                if (predicate(source[index]))
                    builder.Add(selector(source[index]));
            }
            return builder.ToArray();
        }
    }
}

