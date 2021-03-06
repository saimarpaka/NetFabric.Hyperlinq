﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NetFabric.Hyperlinq
{
    public static partial class ArrayExtensions
    {
        public static WhereRefAtEnumerable<TSource> WhereRef<TSource>(this in ArraySegment<TSource> source, PredicateAt<TSource> predicate)
            => new WhereRefAtEnumerable<TSource>(in source, predicate);

        public readonly partial struct WhereRefAtEnumerable<TSource>
            : IValueEnumerable<TSource, WhereRefAtEnumerable<TSource>.DisposableEnumerator>
        {
            readonly ArraySegment<TSource> source;
            readonly PredicateAt<TSource> predicate;

            internal WhereRefAtEnumerable(in ArraySegment<TSource> source, PredicateAt<TSource> predicate)
                => (this.source, this.predicate) = (source, predicate);

            public readonly Enumerator GetEnumerator() 
                => new Enumerator(in this);
            readonly DisposableEnumerator IValueEnumerable<TSource, WhereRefAtEnumerable<TSource>.DisposableEnumerator>.GetEnumerator() 
                => new DisposableEnumerator(in this);
            readonly IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() 
                => new DisposableEnumerator(in this);
            readonly IEnumerator IEnumerable.GetEnumerator() 
                => new DisposableEnumerator(in this);

            public struct Enumerator
            {
                readonly TSource[] source;
                readonly PredicateAt<TSource> predicate;
                readonly int offset;
                readonly int count;
                int index;

                internal Enumerator(in WhereRefAtEnumerable<TSource> enumerable)
                {
                    source = enumerable.source.Array;
                    predicate = enumerable.predicate;
                    offset = enumerable.source.Offset;
                    count = enumerable.source.Count;
                    index = -1;
                }

                [MaybeNull]
                public readonly ref TSource Current
                    => ref source[index + offset]!;

                public bool MoveNext()
                {
                    while (++index < count)
                    {
                        if (predicate(source[index + offset], index))
                            return true;
                    }
                    return false;
                }
            }

            public struct DisposableEnumerator
                : IEnumerator<TSource>
            {
                readonly TSource[] source;
                readonly PredicateAt<TSource> predicate;
                readonly int offset;
                readonly int count;
                int index;

                internal DisposableEnumerator(in WhereRefAtEnumerable<TSource> enumerable)
                {
                    source = enumerable.source.Array;
                    predicate = enumerable.predicate;
                    offset = enumerable.source.Offset;
                    count = enumerable.source.Count;
                    index = -1;
                }

                [MaybeNull]
                public readonly ref TSource Current
                    => ref source[index + offset]!;
                readonly TSource IEnumerator<TSource>.Current
                    => source[index + offset];
                readonly object? IEnumerator.Current
                    => source[index + offset];

                public bool MoveNext()
                {
                    while (++index < count)
                    {
                        if (predicate(source[index + offset], index))
                            return true;
                    }
                    return false;
                }

                [ExcludeFromCodeCoverage]
                public readonly void Reset()
                    => Throw.NotSupportedException();

                public readonly void Dispose() { }
            }

            public int Count()
                => ArrayExtensions.Count<TSource>(source, predicate);

            public bool Any()
                => ArrayExtensions.Any<TSource>(source, predicate);

            public WhereRefAtEnumerable<TSource> WhereRef(Predicate<TSource> predicate)
                => ArrayExtensions.WhereRef<TSource>(source, Utils.Combine(this.predicate, predicate));
            public WhereRefAtEnumerable<TSource> WhereRef(PredicateAt<TSource> predicate)
                => ArrayExtensions.WhereRef<TSource>(source, Utils.Combine(this.predicate, predicate));

            public Option<TSource> ElementAt(int index)
                => ArrayExtensions.ElementAt<TSource>(source, index, predicate);

            public Option<TSource> First()
                => ArrayExtensions.First<TSource>(source, predicate);

            public Option<TSource> Single()
                => ArrayExtensions.Single<TSource>(source, predicate);

            public TSource[] ToArray()
                => ArrayExtensions.ToArray<TSource>(source, predicate);

            public List<TSource> ToList()
                => ArrayExtensions.ToList<TSource>(source, predicate);

            public readonly bool SequenceEqual(IEnumerable<TSource> other, IEqualityComparer<TSource>? comparer = default)
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

