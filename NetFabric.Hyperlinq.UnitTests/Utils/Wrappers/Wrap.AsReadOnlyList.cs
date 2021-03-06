using System;
using System.Collections;
using System.Collections.Generic;

namespace NetFabric.Hyperlinq
{
    public static partial class Wrap
    {
        public static ReadOnlyListWrapper<T> AsReadOnlyList<T>(T[] source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            return new ReadOnlyListWrapper<T>(source);
        }

        public class ReadOnlyListWrapper<T> 
            : ReadOnlyCollectionWrapper<T>
            , IReadOnlyList<T>
        {
            internal ReadOnlyListWrapper(T[] source)
                : base(source)
            { }

            public T this[int index] 
                => source[index];
        }
    }
}