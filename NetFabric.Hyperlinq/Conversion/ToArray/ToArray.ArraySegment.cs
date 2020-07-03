using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace NetFabric.Hyperlinq
{
    public static partial class ArrayExtensions
    {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSource[] ToArray<TSource>(this in ArraySegment<TSource> source)
        {
            var result = new TSource[source.Count];
            ArrayExtensions.Copy(source.Array, source.Offset, result, 0, source.Count);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<TSource> ToArray<TSource>(this in ArraySegment<TSource> source, ArrayPool<TSource> pool)
        {
            var result = new ArraySegment<TSource>(pool.Rent(source.Count), 0, source.Count);
            ArrayExtensions.Copy(source.Array, result.Array, source.Count);
            return result;
        }


#if SPAN_SUPPORTED

        public static IMemoryOwner<TSource> ToArray<TSource>(this in ArraySegment<TSource> source, MemoryPool<TSource> pool)
        {
            var result = pool.RentSliced(source.Count);
            ArrayExtensions.Copy(source.AsSpan(), result.Memory.Span);
            return result;
        }

#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TSource[] ToArray<TSource>(this in ArraySegment<TSource> source, Predicate<TSource> predicate)
        {
            using var builder = new LargeArrayBuilder<TSource>(initialize: true);
            var array = source.Array;
            if (source.Offset == 0 && source.Count == array.Length)
            {
                for (var index = 0; index < array.Length; index++)
                {
                    if (predicate(array[index]))
                        builder.Add(array[index]);
                }
            }
            else
            {
                var end = source.Offset + source.Count;
                for (var index = source.Offset; index < end; index++)
                {
                    if (predicate(array[index]))
                        builder.Add(array[index]);
                }
            }
            return builder.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TSource[] ToArray<TSource>(this in ArraySegment<TSource> source, PredicateAt<TSource> predicate)
        {
            using var builder = new LargeArrayBuilder<TSource>(initialize: true);
            var array = source.Array;
            if (source.Offset == 0)
            {
                if (source.Count == array.Length)
                {
                    for (var index = 0; index < array.Length; index++)
                    {
                        if (predicate(array[index], index))
                            builder.Add(array[index]);
                    }
                }
                else
                {
                    for (var index = 0; index < source.Count; index++)
                    {
                        if (predicate(array[index], index))
                            builder.Add(array[index]);
                    }
                }
            }
            else
            {
                var offset = source.Offset;
                var count = source.Count;
                for (var index = 0; index < count; index++)
                {
                    if (predicate(array[index + offset], index))
                        builder.Add(array[index + offset]);
                }
            }
            return builder.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TResult[] ToArray<TSource, TResult>(this in ArraySegment<TSource> source, NullableSelector<TSource, TResult> selector)
        {
            var result = new TResult[source.Count];
            ArrayExtensions.Copy(source.Array, source.Offset, result, source.Count, selector);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TResult[] ToArray<TSource, TResult>(this in ArraySegment<TSource> source, NullableSelectorAt<TSource, TResult> selector)
        {
            var result = new TResult[source.Count];
            ArrayExtensions.Copy(source.Array, source.Offset, result, source.Count, selector);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TResult[] ToArray<TSource, TResult>(this in ArraySegment<TSource> source, Predicate<TSource> predicate, NullableSelector<TSource, TResult> selector)
        {
            using var builder = new LargeArrayBuilder<TResult>(initialize: true);
            var array = source.Array;
            if (source.Offset == 0 && source.Count == array.Length)
            {
                for (var sourceIndex = 0; sourceIndex < array.Length; sourceIndex++)
                {
                    if (predicate(array[sourceIndex]))
                        builder.Add(selector(array[sourceIndex]));
                }
            }
            else
            {
                var end = source.Offset + source.Count;
                for (var index = source.Offset; index < end; index++)
                {
                    if (predicate(array[index]))
                        builder.Add(selector(array[index]));
                }
            }
            return builder.ToArray();
        }
    }
}

