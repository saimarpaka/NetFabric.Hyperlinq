﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace NetFabric.Hyperlinq
{
    public static partial class Array
    {
        public static WhereIndexEnumerable<TSource> Where<TSource>(this TSource[] source, Func<TSource, int, bool> predicate) 
        {
            if (predicate is null) ThrowHelper.ThrowArgumentNullException(nameof(predicate));

            return new WhereIndexEnumerable<TSource>(source, predicate, 0, source.Length);
        }

        static WhereIndexEnumerable<TSource> Where<TSource>(this TSource[] source, Func<TSource, int, bool> predicate, int skipCount, int takeCount)
            => new WhereIndexEnumerable<TSource>(source, predicate, skipCount, takeCount);

        [GenericsTypeMapping("TEnumerable", typeof(WhereEnumerable<>))]
        [GenericsTypeMapping("TEnumerator", typeof(WhereEnumerable<>.Enumerator))]
        public readonly struct WhereIndexEnumerable<TSource>
            : IValueEnumerable<TSource, WhereIndexEnumerable<TSource>.Enumerator>
        {
            readonly TSource[] source;
            readonly Func<TSource, int, bool> predicate;
            readonly int skipCount;
            readonly int takeCount;

            internal WhereIndexEnumerable(TSource[] source, Func<TSource, int, bool> predicate, int skipCount, int takeCount)
            {
                this.source = source;
                this.predicate = predicate;
                (this.skipCount, this.takeCount) = Utils.SkipTake(source.Length, skipCount, takeCount);
            }

            public Enumerator GetEnumerator() => new Enumerator(in this);
            IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => new Enumerator(in this);
            IEnumerator IEnumerable.GetEnumerator() => new Enumerator(in this);

            public struct Enumerator 
                : IEnumerator<TSource>
            {
                readonly TSource[] source;
                readonly Func<TSource, int, bool> predicate;
                readonly int end;
                int index;

                internal Enumerator(in WhereIndexEnumerable<TSource> enumerable)
                {
                    source = enumerable.source;
                    predicate = enumerable.predicate;
                    end = enumerable.skipCount + enumerable.takeCount;
                    index = enumerable.skipCount - 1;
                }

                public ref TSource Current => ref source[index];
                TSource IEnumerator<TSource>.Current => source[index];
                object IEnumerator.Current => source[index];

                public bool MoveNext()
                {
                    while (++index < end)
                    {
                        if (predicate(source[index], index))
                            return true;
                    }
                    return false;
                }

                void IEnumerator.Reset() => throw new NotSupportedException();

                public void Dispose() { }
            }

            public int Count()
                => source.Count(predicate, skipCount, takeCount);

            public bool Any()
                => source.Any<TSource>(predicate);

            public Array.WhereIndexEnumerable<TSource> Where(Func<TSource, int, bool> predicate)
                => Array.Where<TSource>(source, Utils.Combine(this.predicate, predicate));

            public ref TSource First()
                => ref Array.First<TSource>(source, predicate);
            public ref readonly TSource FirstOrDefault()
                => ref Array.FirstOrDefault<TSource>(source, predicate);

            public ref TSource Single()
                => ref Array.Single<TSource>(source, predicate);
            public ref readonly TSource SingleOrDefault()
                => ref Array.SingleOrDefault<TSource>(source, predicate);

            public List<TSource> ToList()
                => Array.ToList<TSource>(source, predicate, skipCount, takeCount);
        }
    }
}
