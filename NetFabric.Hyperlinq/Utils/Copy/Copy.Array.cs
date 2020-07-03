using System;
using System.Runtime.CompilerServices;

namespace NetFabric.Hyperlinq
{
    static partial class ArrayExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy<TSource>(TSource[] source, TSource[] destination, int count)
            => Array.Copy(source, destination, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy<TSource>(TSource[] source, int sourceOffset, TSource[] destination, int destinationOffset, int count)
            => Array.Copy(source, sourceOffset, destination, destinationOffset, count);

        public static void Copy<TSource, TResult>(TSource[] source, int sourceOffset, TResult[] destination, int count, NullableSelector<TSource, TResult> selector)
        {
            if (sourceOffset == 0)
            {
                if (count == source.Length)
                {
                    for (var index = 0; index < source.Length; index++)
                        destination[index] = selector(source[index])!;
                }
                else
                {
                    for (var index = 0; index < count; index++)
                        destination[index] = selector(source[index])!;
                }
            }
            else
            {
                for (var index = 0; index < count; index++)
                    destination[index] = selector(source[index + sourceOffset])!;
            }
        }

        public static void Copy<TSource, TResult>(TSource[] source, int sourceOffset, TResult[] destination, int count, NullableSelectorAt<TSource, TResult> selector)
        {
            if (sourceOffset == 0)
            {
                if (count == source.Length)
                {
                    for (var index = 0; index < source.Length; index++)
                        destination[index] = selector(source[index], index)!;
                }
                else
                {
                    for (var index = 0; index < count; index++)
                        destination[index] = selector(source[index], index)!;
                }
            }
            else
            {
                for (var index = 0; index < count; index++)
                    destination[index] = selector(source[index + sourceOffset], index)!;
            }
        }
    }
}
