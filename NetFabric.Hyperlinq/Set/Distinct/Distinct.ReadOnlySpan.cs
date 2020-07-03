using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NetFabric.Hyperlinq
{
    public static partial class ArrayExtensions
    {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanDistinctEnumerable<TSource> Distinct<TSource>(
            this ReadOnlySpan<TSource> source, 
            IEqualityComparer<TSource>? comparer = null)
            => new SpanDistinctEnumerable<TSource>(source, comparer);

        public readonly ref struct SpanDistinctEnumerable<TSource>
        {
            readonly ReadOnlySpan<TSource> source;
            readonly IEqualityComparer<TSource>? comparer;

            internal SpanDistinctEnumerable(ReadOnlySpan<TSource> source, IEqualityComparer<TSource>? comparer)
            {
                this.source = source;
                this.comparer = comparer;
            }

            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly Enumerator GetEnumerator() => new Enumerator(in this);

            public ref struct Enumerator
            {
                readonly ReadOnlySpan<TSource> source;
                readonly Set<TSource> set;
                int index;

                internal Enumerator(in SpanDistinctEnumerable<TSource> enumerable)
                {
                    source = enumerable.source;
                    set = new Set<TSource>(enumerable.comparer);
                    index = -1;
                }

                public ref readonly TSource Current 
                    => ref source[index];

                public bool MoveNext()
                {
                    while (++index < source.Length)
                    {
                        if (set.Add(source[index]))
                            return true;
                    }

                    Dispose();
                    return false;
                }

                public void Dispose() 
                    => set.Dispose();
            }

            readonly Set<TSource> GetSet() 
            {
                var set = new Set<TSource>(comparer);
                for (var index = 0; index < source.Length; index++)
                    _ = set.Add(source[index]);
                return set;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly int Count()
                => source.Length == 0
                    ? 0
                    : GetSet().Count;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool Any()
                => source.Length != 0;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly TSource[] ToArray()
                => source.Length == 0
                    ? Array.Empty<TSource>()
                    : GetSet().ToArray();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly ArraySegment<TSource> ToArray(ArrayPool<TSource> pool)
                => source.Length == 0
                    ? new ArraySegment<TSource>(pool.Rent(0), 0, 0)
                    : GetSet().ToArray(pool);

#if SPAN_SUPPORTED

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly IMemoryOwner<TSource> ToArray(MemoryPool<TSource> pool)
                => source.Length == 0
                    ? pool.Rent(0)
                    : GetSet().ToArray(pool);

#endif

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly List<TSource> ToList()
                => source.Length == 0
                    ? new List<TSource>()
                    : GetSet().ToList();

            public readonly bool SequenceEqual(IEnumerable<TSource> other, IEqualityComparer<TSource>? comparer = null)
            {
                comparer ??= EqualityComparer<TSource>.Default;

                var enumerator = GetEnumerator();
                using var otherEnumerator = other.GetEnumerator();
                while (true)
                {
                    var thisEnded = !enumerator.MoveNext();
                    var otherEnded = !otherEnumerator.MoveNext();

                    if (thisEnded != otherEnded)
                        return false;

                    if (thisEnded)
                        return true;

                    if (!comparer.Equals(enumerator.Current, otherEnumerator.Current))
                        return false;
                }
            }
        }
    }
}

