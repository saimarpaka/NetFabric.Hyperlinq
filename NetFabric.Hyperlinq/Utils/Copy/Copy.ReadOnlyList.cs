using System;
using System.Collections.Generic;

namespace NetFabric.Hyperlinq
{
    static partial class ReadOnlyListExtensions
    {
        public static void Copy<TList, TSource>(TList source, int sourceOffset, TSource[] destination, int destinationOffset, int count)
            where TList : IReadOnlyList<TSource>
        {
            if (count != 0)
            {
                if (sourceOffset == 0)
                {
                    if (count == source.Count && source is ICollection<TSource> collection)
                    {
                        collection.CopyTo(destination, destinationOffset);
                    }
                    else
                    {
                        if (destinationOffset == 0)
                        {
                            for (var index = 0; index < count; index++)
                                destination[index] = source[index];
                        }
                        else
                        {
                            for (var index = 0; index < count; index++)
                                destination[index + destinationOffset] = source[index];
                        }
                    }
                }
                else
                {
                    if (destinationOffset == 0)
                    {
                        for (var index = 0; index < count; index++)
                            destination[index] = source[index + sourceOffset];
                    }
                    else
                    {
                        for (var index = 0; index < count; index++)
                            destination[index + destinationOffset] = source[index + sourceOffset];
                    }
                }
            }
        }

#if SPAN_SUPPORTED

        public static void Copy<TList, TSource>(TList source, int sourceOffset, Span<TSource> destination, int count)
            where TList : IReadOnlyList<TSource>
        {
            if (sourceOffset == 0)
            {
                for (var index = 0; index < count; index++)
                    destination[index] = source[index];
            }
            else
            {
                for (var index = 0; index < count; index++)
                    destination[index] = source[index + sourceOffset];
            }
        }
#endif

        public static void Copy<TList, TSource, TResult>(TList source, int offset, TResult[] destination, int destinationOffset, int count, NullableSelector<TSource, TResult> selector)
            where TList : IReadOnlyList<TSource>
        {
            if (destinationOffset == 0)
            {
                if (offset == 0)
                {
                    for (var index = 0; index < count; index++)
                        destination[index] = selector(source[index])!;
                }
                else
                {
                    for (var index = 0; index < count; index++)
                        destination[index] = selector(source[index + offset])!;
                }
            }
            else
            {
                if (offset == 0)
                {
                    for (var index = 0; index < count; index++)
                        destination[index + destinationOffset] = selector(source[index])!;
                }
                else
                {
                    for (var index = 0; index < count; index++)
                        destination[index + destinationOffset] = selector(source[index + offset])!;
                }
            }
        }

        public static void Copy<TList, TSource, TResult>(TList source, int offset, TResult[] destination, int destinationOffset, int count, NullableSelectorAt<TSource, TResult> selector)
            where TList : IReadOnlyList<TSource>
        {
            if (destinationOffset == 0)
            {
                if (offset == 0)
                {
                    for (var index = 0; index < count; index++)
                        destination[index] = selector(source[index], index)!;
                }
                else
                {
                    for (var index = 0; index < count; index++)
                        destination[index] = selector(source[index + offset], index)!;
                }
            }
            else
            {
                if (offset == 0)
                {
                    for (var index = 0; index < count; index++)
                        destination[index + destinationOffset] = selector(source[index], index)!;
                }
                else
                {
                    for (var index = 0; index < count; index++)
                        destination[index + destinationOffset] = selector(source[index + offset], index)!;
                }
            }
        }
    }
}
