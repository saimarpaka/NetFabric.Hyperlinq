using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NetFabric.Hyperlinq
{
    public static partial class SpanExtensions
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly TSource First<TSource>(this ReadOnlySpan<TSource> source)
        {
            if (source.Length == 0) Throw.EmptySequence<TSource>();

            return ref source[0];
        }

        [Pure]
        public static ref readonly TSource First<TSource>(this ReadOnlySpan<TSource> source, Predicate<TSource> predicate)
        {
            if (predicate is null) Throw.ArgumentNullException(nameof(predicate));

            var length = source.Length;
            for (var index = 0; index < length; index++)
            {
                if (predicate(source[index]))
                    return ref source[index];
            }
            Throw.EmptySequence<TSource>();
            return ref source[0];
        }

        [Pure]
        public static ref readonly TSource First<TSource>(this ReadOnlySpan<TSource> source, PredicateAt<TSource> predicate)
        {
            if (predicate is null) Throw.ArgumentNullException(nameof(predicate));

            var length = source.Length;
            for (var index = 0; index < length; index++)
            {
                if (predicate(source[index], index))
                    return ref source[index];
            }
            Throw.EmptySequence<TSource>();
            return ref source[0];
        }

        [Pure]
        public static ref readonly TSource First<TSource>(this ReadOnlySpan<TSource> source, PredicateAt<TSource> predicate, out int index)
        {
            if (predicate is null) Throw.ArgumentNullException(nameof(predicate));

            var length = source.Length;
            for (index = 0; index < length; index++)
            {
                if (predicate(source[index], index))
                    return ref source[index];
            }
            Throw.EmptySequence<TSource>();
            return ref source[0];
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: MaybeNull]
        public static ref readonly TSource FirstOrDefault<TSource>(this ReadOnlySpan<TSource> source)
        {
            if (source.Length == 0) return ref Default<TSource>.Value;

            return ref source[0];
        }

        [Pure]
        [return: MaybeNull]
        public static ref readonly TSource FirstOrDefault<TSource>(this ReadOnlySpan<TSource> source, Predicate<TSource> predicate)
        {
            if (predicate is null) Throw.ArgumentNullException(nameof(predicate));

            var length = source.Length;
            for (var index = 0; index < length; index++)
            {
                if (predicate(source[index]))
                    return ref source[index];
            }
            return ref Default<TSource>.Value;
        }

        [Pure]
        [return: MaybeNull]
        public static ref readonly TSource FirstOrDefault<TSource>(this ReadOnlySpan<TSource> source, PredicateAt<TSource> predicate)
        {
            if (predicate is null) Throw.ArgumentNullException(nameof(predicate));

            var length = source.Length;
            for (var index = 0; index < length; index++)
            {
                if (predicate(source[index], index))
                    return ref source[index];
            }
            return ref Default<TSource>.Value;
        }

        [Pure]
        [return: MaybeNull]
        public static ref readonly TSource FirstOrDefault<TSource>(this ReadOnlySpan<TSource> source, PredicateAt<TSource> predicate, out int index)
        {
            if (predicate is null) Throw.ArgumentNullException(nameof(predicate));

            var length = source.Length;
            for (index = 0; index < length; index++)
            {
                if (predicate(source[index], index))
                    return ref source[index];
            }
            index = -1;
            return ref Default<TSource>.Value;
        }
    }
}
