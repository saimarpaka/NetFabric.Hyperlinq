﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NetFabric.Hyperlinq
{
    public static partial class ArrayExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SpanWhereRefAtEnumerable<TSource> WhereRef<TSource>(this Span<TSource> source, PredicateAt<TSource> predicate)
        {
            if (predicate is null)
                Throw.ArgumentNullException(nameof(predicate));

            return new SpanWhereRefAtEnumerable<TSource>(source, predicate);
        }

        public readonly ref struct SpanWhereRefAtEnumerable<TSource>
        {
            internal readonly Span<TSource> source;
            internal readonly PredicateAt<TSource> predicate;

            internal SpanWhereRefAtEnumerable(in Span<TSource> source, PredicateAt<TSource> predicate)
            {
                this.source = source;
                this.predicate = predicate;
            }


            public readonly Enumerator GetEnumerator() => new Enumerator(in this);

            public ref struct Enumerator
            {
                readonly Span<TSource> source;
                readonly PredicateAt<TSource> predicate;
                int index;

                internal Enumerator(in SpanWhereRefAtEnumerable<TSource> enumerable)
                {
                    source = enumerable.source;
                    predicate = enumerable.predicate;
                    index = -1;
                }

                public readonly TSource Current
                    => source[index];

                public bool MoveNext()
                {
                    while (++index < source.Length)
                    {
                        if (predicate(source[index], index))
                            return true;
                    }
                    return false;
                }
            }

            public int Count()
                => ArrayExtensions.Count(source, predicate);

            public bool Any()
                => ArrayExtensions.Any(source, predicate);

            public SpanWhereRefAtEnumerable<TSource> WhereRef(Predicate<TSource> predicate)
                => WhereRef<TSource>(source, Utils.Combine(this.predicate, predicate));

            public SpanWhereRefAtEnumerable<TSource> WhereRef(PredicateAt<TSource> predicate)
                => WhereRef<TSource>(source, Utils.Combine(this.predicate, predicate));

            public Option<TSource> ElementAt(int index)
                => ArrayExtensions.ElementAt<TSource>(source, index, predicate);

            public Option<TSource> First()
                => ArrayExtensions.First<TSource>(source, predicate);

            public Option<TSource> Single()
                => ArrayExtensions.Single<TSource>(source, predicate);

            public TSource[] ToArray()
                => ArrayExtensions.ToArray(source, predicate);

            public List<TSource> ToList()
                => ArrayExtensions.ToList(source, predicate);

            public bool SequenceEqual(IEnumerable<TSource> other, IEqualityComparer<TSource>? comparer = null)
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

