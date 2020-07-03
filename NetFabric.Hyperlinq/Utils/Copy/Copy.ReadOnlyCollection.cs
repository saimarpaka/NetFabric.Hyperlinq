using System.Collections.Generic;

namespace NetFabric.Hyperlinq
{
    static partial class ReadOnlyCollectionExtensions
    {
        public static void Copy<TEnumerable, TSource>(TEnumerable source, TSource[] destination, int destinationOffset)
            where TEnumerable : IReadOnlyCollection<TSource>
        {
            if (source.Count != 0)
            {
                switch (source)
                {
                    case ICollection<TSource> collection:
                        collection.CopyTo(destination, destinationOffset);
                        break;

                    default:
                        {
                            using var enumerator = source.GetEnumerator();
                            checked
                            {
                                for (var index = destinationOffset; enumerator.MoveNext(); index++)
                                    destination[index] = enumerator.Current;
                            }
                        }
                        break;
                }
            }
        }

        public static void Copy<TEnumerable, TSource, TResult>(TEnumerable source, TResult[] destination, NullableSelector<TSource, TResult> selector)
            where TEnumerable : IReadOnlyCollection<TSource>
        {
            if (source.Count != 0)
            {
                using var enumerator = source.GetEnumerator();
                checked
                {
                    for (var index = 0; enumerator.MoveNext(); index++)
                        destination[index] = selector(enumerator.Current)!;
                }
            }
        }

        public static void Copy<TEnumerable, TSource, TResult>(TEnumerable source, TResult[] destination, NullableSelectorAt<TSource, TResult> selector)
            where TEnumerable : IReadOnlyCollection<TSource>
        {
            if (source.Count != 0)
            {
                using var enumerator = source.GetEnumerator();
                checked
                {
                    for (var index = 0; enumerator.MoveNext(); index++)
                        destination[index] = selector(enumerator.Current, index)!;
                }
            }
        }
    }
}
