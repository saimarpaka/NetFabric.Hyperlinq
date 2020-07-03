using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NetFabric.Hyperlinq
{
    public static partial class ValueReadOnlyCollectionExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSource[] ToArray<TEnumerable, TEnumerator, TSource>(this TEnumerable source)
            where TEnumerable : IValueReadOnlyCollection<TSource, TEnumerator>
            where TEnumerator : struct, IEnumerator<TSource>
        {
            var array = new TSource[source.Count];
            ValueReadOnlyCollectionExtensions.Copy<TEnumerable, TEnumerator, TSource>(source, array);
            return array;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TResult[] ToArray<TEnumerable, TEnumerator, TSource, TResult>(this TEnumerable source, NullableSelector<TSource, TResult> selector)
            where TEnumerable : IValueReadOnlyCollection<TSource, TEnumerator>
            where TEnumerator : struct, IEnumerator<TSource>
        {
            var array = new TResult[source.Count];
            ValueReadOnlyCollectionExtensions.Copy<TEnumerable, TEnumerator, TSource, TResult>(source, array, selector);
            return array;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TResult[] ToArray<TEnumerable, TEnumerator, TSource, TResult>(this TEnumerable source, NullableSelectorAt<TSource, TResult> selector)
            where TEnumerable : IValueReadOnlyCollection<TSource, TEnumerator>
            where TEnumerator : struct, IEnumerator<TSource>
        {
            var array = new TResult[source.Count];
            ValueReadOnlyCollectionExtensions.Copy<TEnumerable, TEnumerator, TSource, TResult>(source, array, selector);
            return array;
        }
    }
}
