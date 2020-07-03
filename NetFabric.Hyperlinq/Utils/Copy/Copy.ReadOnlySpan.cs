using System;
using System.Runtime.CompilerServices;

namespace NetFabric.Hyperlinq
{
    static partial class ArrayExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy<TSource>(ReadOnlySpan<TSource> source, Span<TSource> destination) 
            => source.CopyTo(destination);

        public static void Copy<TSource, TResult>(ReadOnlySpan<TSource> source, Span<TResult> destination, NullableSelector<TSource, TResult> selector)
        {
            for (var index = 0; index < source.Length; index++)
                destination[index] = selector(source[index])!;
        }

        public static void Copy<TSource, TResult>(ReadOnlySpan<TSource> source, Span<TResult> destination, NullableSelectorAt<TSource, TResult> selector)
        {
            for (var index = 0; index < source.Length; index++)
                destination[index] = selector(source[index], index)!;
        }
    }
}
