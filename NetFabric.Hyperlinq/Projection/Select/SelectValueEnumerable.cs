﻿using System;
using System.Collections.Generic;

namespace NetFabric.Hyperlinq
{
    public static partial class ValueEnumerable
    {
        public static SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult> Select<TEnumerable, TEnumerator, TSource, TResult>(
            this TEnumerable source, 
            Func<TSource, TResult> selector)
            where TEnumerable : IValueEnumerable<TSource, TEnumerator>
            where TEnumerator : struct, IValueEnumerator<TSource>
        {
            if (source == null) ThrowHelper.ThrowArgumentNullException(nameof(source));
            if (selector is null) ThrowHelper.ThrowArgumentNullException(nameof(selector));

            return new SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult>(in source, selector);
        }

        public readonly struct SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult> 
            : IValueEnumerable<TResult, SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult>.ValueEnumerator>
            where TEnumerable : IValueEnumerable<TSource, TEnumerator>
            where TEnumerator : struct, IValueEnumerator<TSource>
        {
            readonly TEnumerable source;
            readonly Func<TSource, TResult> selector;

            internal SelectEnumerable(in TEnumerable source, Func<TSource, TResult> selector)
            {
                this.source = source;
                this.selector = selector;
            }

            public Enumerator GetEnumerator() => new Enumerator(in this);
            public ValueEnumerator GetValueEnumerator() => new ValueEnumerator(in this);

            public struct Enumerator
                : IDisposable
            {
                TEnumerator enumerator;
                readonly Func<TSource, TResult> selector;
                TSource current;

                internal Enumerator(in SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult> enumerable)
                {
                    enumerator = enumerable.source.GetValueEnumerator();
                    selector = enumerable.selector;
                    current = default;
                }

                public TResult Current => selector(current);

                public bool MoveNext() => enumerator.TryMoveNext(out current);

                public void Dispose() => enumerator.Dispose();
            }

            public struct ValueEnumerator
                : IValueEnumerator<TResult>
            {
                TEnumerator enumerator;
                readonly Func<TSource, TResult> selector;

                internal ValueEnumerator(in SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult> enumerable)
                {
                    enumerator = enumerable.source.GetValueEnumerator();
                    selector = enumerable.selector;
                }

                public bool TryMoveNext(out TResult current)
                {
                    if(enumerator.TryMoveNext(out var temp))
                    {
                        current = selector(temp);
                        return true;
                    }
                    current = default;
                    return false;
                }

                public bool TryMoveNext() => enumerator.TryMoveNext();

                public void Dispose() => enumerator.Dispose();
            }

            public int Count()
                => ValueEnumerable.Count<TEnumerable, TEnumerator, TSource>(source);

            public ValueEnumerable.SelectEnumerable<SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult>, ValueEnumerator, TResult, TSelectorResult> Select<TSelectorResult>(Func<TResult, TSelectorResult> selector)
                 => ValueEnumerable.Select<SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult>, ValueEnumerator, TResult, TSelectorResult>(this, selector);

            public ValueEnumerable.WhereEnumerable<SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult>, ValueEnumerator, TResult> Where(Func<TResult, bool> predicate)
                => ValueEnumerable.Where<SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult>, ValueEnumerator, TResult>(this, predicate);

            public TResult First()
                => ValueEnumerable.First<SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult>, ValueEnumerator, TResult>(this);
            public TResult First(Func<TResult, bool> predicate)
                => ValueEnumerable.First<SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult>, ValueEnumerator, TResult>(this, predicate);

            public TResult FirstOrDefault()
                => ValueEnumerable.FirstOrDefault<SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult>, ValueEnumerator, TResult>(this);
            public TResult FirstOrDefault(Func<TResult, bool> predicate)
                => ValueEnumerable.FirstOrDefault<SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult>, ValueEnumerator, TResult>(this, predicate);

            public TResult Single()
                => ValueEnumerable.Single<SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult>, ValueEnumerator, TResult>(this);
            public TResult Single(Func<TResult, bool> predicate)
                => ValueEnumerable.Single<SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult>, ValueEnumerator, TResult>(this, predicate);

            public TResult SingleOrDefault()
                => ValueEnumerable.SingleOrDefault<SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult>, ValueEnumerator, TResult>(this);
            public TResult SingleOrDefault(Func<TResult, bool> predicate)
                => ValueEnumerable.SingleOrDefault<SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult>, ValueEnumerator, TResult>(this, predicate);

            public IEnumerable<TResult> AsEnumerable()
                => ValueEnumerable.AsEnumerable<SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult>, ValueEnumerator, TResult>(this);

            public SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult> AsValueEnumerable()
                => this;

            public TResult[] ToArray()
                => ValueEnumerable.ToArray<SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult>, ValueEnumerator, TResult>(this);

            public List<TResult> ToList()
                => ValueEnumerable.ToList<SelectEnumerable<TEnumerable, TEnumerator, TSource, TResult>, ValueEnumerator, TResult>(this);
        }
    }
}

