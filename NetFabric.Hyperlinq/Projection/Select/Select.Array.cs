﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NetFabric.Hyperlinq
{
    public static partial class ArrayExtensions
    {
#if SPAN_SUPPORTED

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MemorySelectEnumerable<TSource, TResult> Select<TSource, TResult>(this TSource[] source, Selector<TSource, TResult> selector)
            => Select(source.AsMemory(), selector);

#else

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SelectEnumerable<TSource, TResult> Select<TSource, TResult>(
            this TSource[] source,
            Selector<TSource, TResult> selector)
        {
            if (selector is null)
                Throw.ArgumentNullException(nameof(selector));

            return new SelectEnumerable<TSource, TResult>(in source, selector, 0, source.Length);
        }

        static SelectEnumerable<TSource, TResult> Select<TSource, TResult>(
            this TSource[] source,
            Selector<TSource, TResult> selector,
            int skipCount, int Count)
            => new SelectEnumerable<TSource, TResult>(source, selector, skipCount, Count);

        [GeneratorMapping("TSource", "TResult")]
        public readonly partial struct SelectEnumerable<TSource, TResult>
            : IValueReadOnlyList<TResult, SelectEnumerable<TSource, TResult>.DisposableEnumerator>
            , IList<TResult>
        {
            readonly TSource[] source;
            readonly Selector<TSource, TResult> selector;
            readonly int skipCount;

            internal SelectEnumerable(in TSource[] source, Selector<TSource, TResult> selector, int skipCount, int takeCount)
            {
                this.source = source;
                this.selector = selector;
                (this.skipCount, Count) = Utils.SkipTake(source.Length, skipCount, takeCount);
            }

            public readonly int Count { get; }

            [MaybeNull]
            public readonly TResult this[int index]
            {
                get
                {
                    if (index < 0 || index >= Count)
                        Throw.ArgumentOutOfRangeException(nameof(index));

                    return selector(source[index + skipCount]);
                }
            }
            TResult IReadOnlyList<TResult>.this[int index]
                => this[index];
            TResult IList<TResult>.this[int index]
            {
                get => this[index];
                set => Throw.NotSupportedException();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly Enumerator GetEnumerator() => new Enumerator(in this);
            readonly DisposableEnumerator IValueEnumerable<TResult, SelectEnumerable<TSource, TResult>.DisposableEnumerator>.GetEnumerator() => new DisposableEnumerator(in this);
            readonly IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => new DisposableEnumerator(in this);
            readonly IEnumerator IEnumerable.GetEnumerator() => new DisposableEnumerator(in this);

            bool ICollection<TResult>.IsReadOnly
                => true;

            void ICollection<TResult>.CopyTo(TResult[] array, int arrayIndex)
            {
                for (var index = 0; index < Count; index++)
                    array[index + arrayIndex] = selector(source[index + skipCount]);
            }
            void ICollection<TResult>.Add(TResult item)
                => Throw.NotSupportedException();
            void ICollection<TResult>.Clear()
                => Throw.NotSupportedException();
            bool ICollection<TResult>.Contains(TResult item)
                => ArrayExtensions.Contains<TSource, TResult>(source, item, selector, skipCount, Count);
            bool ICollection<TResult>.Remove(TResult item)
                => Throw.NotSupportedException<bool>();
            int IList<TResult>.IndexOf(TResult item)
            {
                if (default(TResult) is object)
                {
                    for (var index = skipCount; index < Count; index++)
                    {
                        if (EqualityComparer<TResult>.Default.Equals(selector(source[index]), item))
                            return index - skipCount;
                    }
                }
                else
                {
                    var defaultComparer = EqualityComparer<TResult>.Default;
                    for (var index = skipCount; index < Count; index++)
                    {
                        if (defaultComparer.Equals(selector(source[index]), item))
                            return index - skipCount;
                    }
                }
                return -1;
            }
            void IList<TResult>.Insert(int index, TResult item)
                => Throw.NotSupportedException();
            void IList<TResult>.RemoveAt(int index)
                => Throw.NotSupportedException();

            public struct Enumerator
            {
                readonly TSource[] source;
                readonly Selector<TSource, TResult> selector;
                readonly int end;
                int index;

                internal Enumerator(in SelectEnumerable<TSource, TResult> enumerable)
                {
                    source = enumerable.source;
                    selector = enumerable.selector;
                    end = enumerable.skipCount + enumerable.Count;
                    index = enumerable.skipCount - 1;
                }

                [MaybeNull]
                public readonly TResult Current
                    => selector(source[index]);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool MoveNext()
                    => ++index < end;
            }

            public struct DisposableEnumerator
                : IEnumerator<TResult>
            {
                readonly TSource[] source;
                readonly Selector<TSource, TResult> selector;
                readonly int end;
                int index;

                internal DisposableEnumerator(in SelectEnumerable<TSource, TResult> enumerable)
                {
                    source = enumerable.source;
                    selector = enumerable.selector;
                    end = enumerable.skipCount + enumerable.Count;
                    index = enumerable.skipCount - 1;
                }

                [MaybeNull]
                public readonly TResult Current
                    => selector(source[index]);
                readonly TResult IEnumerator<TResult>.Current
                    => selector(source[index]);
                readonly object? IEnumerator.Current
                    => selector(source[index]);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool MoveNext()
                    => ++index < end;

                [ExcludeFromCodeCoverage]
                public readonly void Reset()
                    => Throw.NotSupportedException();

                public readonly void Dispose() { }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public SelectEnumerable<TSource, TResult> Skip(int count)
                => new SelectEnumerable<TSource, TResult>(source, selector, skipCount + count, Count);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public SelectEnumerable<TSource, TResult> Take(int count)
                => new SelectEnumerable<TSource, TResult>(source, selector, skipCount, Math.Min(Count, count));


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Any()
                => Count != 0;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public SelectEnumerable<TSource, TSelectorResult> Select<TSelectorResult>(Selector<TResult, TSelectorResult> selector)
                => ArrayExtensions.Select<TSource, TSelectorResult>(source, Utils.Combine(this.selector, selector), skipCount, Count);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public SelectAtEnumerable<TSource, TSelectorResult> Select<TSelectorResult>(SelectorAt<TResult, TSelectorResult> selector)
                => ArrayExtensions.Select<TSource, TSelectorResult>(source, Utils.Combine(this.selector, selector), skipCount, Count);


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Option<TResult> ElementAt(int index)
                => ArrayExtensions.ElementAt<TSource, TResult>(source, index, selector, skipCount, Count);


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Option<TResult> First()
                => ArrayExtensions.First<TSource, TResult>(source, selector, skipCount, Count);


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Option<TResult> Single()
                => ArrayExtensions.Single<TSource, TResult>(source, selector, skipCount, Count);


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TResult[] ToArray()
                => ArrayExtensions.ToArray<TSource, TResult>(source, selector, skipCount, Count);


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public List<TResult> ToList()
                => ArrayExtensions.ToList<TSource, TResult>(source, selector, skipCount, Count);

            public readonly bool SequenceEqual(IEnumerable<TResult> other, IEqualityComparer<TResult>? comparer = default)
            {
                comparer ??= EqualityComparer<TResult>.Default;

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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count<TSource, TResult>(this SelectEnumerable<TSource, TResult> source)
            => source.Count;

#endif
    }
}

