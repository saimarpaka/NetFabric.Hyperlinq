using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NetFabric.Hyperlinq
{
    public static partial class ArrayExtensions
    {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSource[] ToArray<TSource>(this TSource[] source)
        {
            var result = new TSource[source.Length];
            ArrayExtensions.Copy(source, result, source.Length);
            return result;
        }

        public static ArraySegment<TSource> ToArray<TSource>(this TSource[] source, ArrayPool<TSource> pool)
        {
            var result = new ArraySegment<TSource>(pool.Rent(source.Length), 0, source.Length);
            ArrayExtensions.Copy(source, result.Array, source.Length);
            return result;
        }

#if SPAN_SUPPORTED

        public static IMemoryOwner<TSource> ToArray<TSource>(this TSource[] source, MemoryPool<TSource> pool)
        {
            var result = pool.RentSliced(source.Length);
            ArrayExtensions.Copy(source.AsSpan(), result.Memory.Span);
            return result;
        }

#endif
    }
}

