using System;
using System.Runtime.CompilerServices;

namespace NetFabric.Hyperlinq
{
    static partial class ArrayExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy<TSource>(in ArraySegment<TSource> source, TSource[] destination, int destinationOffset = 0)
            => Array.Copy(source.Array, source.Offset, destination, destinationOffset, source.Count);

        public static void Copy<TSource, TResult>(in ArraySegment<TSource> source, TResult[] destination, int destinationOffset, NullableSelector<TSource, TResult> selector)
        {
            var array = source.Array;
            if (destinationOffset == 0)
            {
                if (source.Offset == 0)
                {
                    if (source.Count == array.Length)
                    {
                        for (var index = 0; index < array.Length; index++)
                            destination[index] = selector(array[index])!;
                    }
                    else
                    {
                        for (var index = 0; index < source.Count; index++)
                            destination[index] = selector(array[index])!;
                    }
                }
                else
                {
                    for (var index = 0; index < source.Count; index++)
                        destination[index] = selector(array[index + source.Offset])!;
                }
            }
            else
            {
                if (source.Offset == 0)
                {
                    if (source.Count == array.Length)
                    {
                        for (var index = 0; index < array.Length; index++)
                            destination[index + destinationOffset] = selector(array[index])!;
                    }
                    else
                    {
                        for (var index = 0; index < source.Count; index++)
                            destination[index + destinationOffset] = selector(array[index])!;
                    }
                }
                else
                {
                    for (var index = 0; index < source.Count; index++)
                        destination[index + destinationOffset] = selector(array[index + source.Offset])!;
                }
            }
        }

        public static void Copy<TSource, TResult>(in ArraySegment<TSource> source, TResult[] destination, int destinationOffset, NullableSelectorAt<TSource, TResult> selector)
        {
            var array = source.Array;
            if (destinationOffset == 0)
            {
                if (source.Offset == 0)
                {
                    if (source.Count == array.Length)
                    {
                        for (var index = 0; index < array.Length; index++)
                            destination[index] = selector(array[index], index)!;
                    }
                    else
                    {
                        for (var index = 0; index < source.Count; index++)
                            destination[index] = selector(array[index], index)!;
                    }
                }
                else
                {
                    for (var index = 0; index < source.Count; index++)
                        destination[index] = selector(array[index + source.Offset], index)!;
                }
            }
            else
            {
                if (source.Offset == 0)
                {
                    if (source.Count == array.Length)
                    {
                        for (var index = 0; index < array.Length; index++)
                            destination[index + destinationOffset] = selector(array[index], index)!;
                    }
                    else
                    {
                        for (var index = 0; index < source.Count; index++)
                            destination[index + destinationOffset] = selector(array[index], index)!;
                    }
                }
                else
                {
                    for (var index = 0; index < source.Count; index++)
                        destination[index + destinationOffset] = selector(array[index + source.Offset], index)!;
                }
            }
        }
    }
}
