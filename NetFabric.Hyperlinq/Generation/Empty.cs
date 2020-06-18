using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NetFabric.Hyperlinq
{
    public static partial class ValueEnumerable
    {
        
        public static EmptyEnumerable<TSource> Empty<TSource>() =>
            EmptyEnumerable<TSource>.Instance;

        public partial class EmptyEnumerable<TSource>
            : IValueReadOnlyList<TSource, EmptyEnumerable<TSource>.DisposableEnumerator>
            , IList<TSource>
        {
            public static readonly EmptyEnumerable<TSource> Instance = new EmptyEnumerable<TSource>();

            private EmptyEnumerable() { }

            public int Count 
                => 0;

            public TSource this[int index]
                => Throw.IndexOutOfRangeException<TSource>();
            TSource IList<TSource>.this[int index]
            {
                get => this[index];
                [ExcludeFromCodeCoverage]
                set => Throw.NotSupportedException();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator GetEnumerator() => new Enumerator();
            DisposableEnumerator IValueEnumerable<TSource, EmptyEnumerable<TSource>.DisposableEnumerator>.GetEnumerator() => new DisposableEnumerator();
            IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => new DisposableEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => new DisposableEnumerator();

            bool ICollection<TSource>.IsReadOnly  
                => true;

            public void CopyTo(TSource[] array, int arrayIndex) 
            {
                // nothing to do 
            }

            public bool Contains(TSource item)
                => false;

            public int IndexOf(TSource item)
                => -1;

            [ExcludeFromCodeCoverage]
            void ICollection<TSource>.Add(TSource item) 
                => Throw.NotSupportedException();
            [ExcludeFromCodeCoverage]
            void ICollection<TSource>.Clear() 
                => Throw.NotSupportedException();
            [ExcludeFromCodeCoverage]
            bool ICollection<TSource>.Remove(TSource item) 
                => Throw.NotSupportedException<bool>();
            [ExcludeFromCodeCoverage]
            void IList<TSource>.Insert(int index, TSource item)
                => Throw.NotSupportedException();
            [ExcludeFromCodeCoverage]
            void IList<TSource>.RemoveAt(int index)
                => Throw.NotSupportedException();

            public readonly struct Enumerator
            {
                [ExcludeFromCodeCoverage]
                [MaybeNull]                
                public readonly TSource Current
                    => default;

                public readonly bool MoveNext() 
                    => false;
            }

            public readonly struct DisposableEnumerator
                : IEnumerator<TSource>
            {
                [ExcludeFromCodeCoverage]
                [MaybeNull]
                public readonly TSource Current
                    => default;

                [ExcludeFromCodeCoverage]
                readonly TSource IEnumerator<TSource>.Current
                    => default!;

                [ExcludeFromCodeCoverage]
                readonly object? IEnumerator.Current 
                    => default;

                public readonly bool MoveNext()
                    => false;

                [ExcludeFromCodeCoverage]
                public readonly void Reset() { }

                public readonly void Dispose() { }
            }
        }
    }
}

