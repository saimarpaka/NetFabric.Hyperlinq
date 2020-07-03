﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFabric.Hyperlinq
{
    public static partial class AsyncValueEnumerableExtensions
    {
        
        public static async ValueTask<TSource[]> ToArrayAsync<TEnumerable, TEnumerator, TSource>(this TEnumerable source, CancellationToken cancellationToken = default)
            where TEnumerable : IAsyncValueEnumerable<TSource, TEnumerator>
            where TEnumerator : struct, IAsyncEnumerator<TSource>
        {
            using var builder = new LargeArrayBuilder<TSource>(initialize: true);
            var enumerator = source.GetAsyncEnumerator();
            await using (enumerator.ConfigureAwait(false))
            {
                while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    builder.Add(enumerator.Current);
                }
            }
            return builder.ToArray();
        }

        static async ValueTask<TSource[]> ToArrayAsync<TEnumerable, TEnumerator, TSource>(this TEnumerable source, AsyncPredicate<TSource> predicate, CancellationToken cancellationToken)
            where TEnumerable : IAsyncValueEnumerable<TSource, TEnumerator>
            where TEnumerator : struct, IAsyncEnumerator<TSource>
        {
            using var builder = new LargeArrayBuilder<TSource>(initialize: true);
            var enumerator = source.GetAsyncEnumerator();
            await using (enumerator.ConfigureAwait(false))
            {
                while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var item = enumerator.Current;
                    if (await predicate(item, cancellationToken).ConfigureAwait(false))
                        builder.Add(item);
                }
            }
            return builder.ToArray();
        }

        static async ValueTask<TSource[]> ToArrayAsync<TEnumerable, TEnumerator, TSource>(this TEnumerable source, AsyncPredicateAt<TSource> predicate, CancellationToken cancellationToken)
            where TEnumerable : IAsyncValueEnumerable<TSource, TEnumerator>
            where TEnumerator : struct, IAsyncEnumerator<TSource>
        {
            using var builder = new LargeArrayBuilder<TSource>(initialize: true);
            var enumerator = source.GetAsyncEnumerator();
            await using (enumerator.ConfigureAwait(false))
            {
                checked
                {
                    for (var index = 0; await enumerator.MoveNextAsync().ConfigureAwait(false); index++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var item = enumerator.Current;
                        if (await predicate(item, index, cancellationToken).ConfigureAwait(false))
                            builder.Add(item);
                    }
                }
            }
            return builder.ToArray();
        }


        static async ValueTask<TResult[]> ToArrayAsync<TEnumerable, TEnumerator, TSource, TResult>(this TEnumerable source, AsyncSelector<TSource, TResult> selector, CancellationToken cancellationToken)
            where TEnumerable : IAsyncValueEnumerable<TSource, TEnumerator>
            where TEnumerator : struct, IAsyncEnumerator<TSource>
        {
            using var builder = new LargeArrayBuilder<TResult>(initialize: true);
            var enumerator = source.GetAsyncEnumerator();
            await using (enumerator.ConfigureAwait(false))
            {
                while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    builder.Add(await selector(enumerator.Current, cancellationToken).ConfigureAwait(false));
                }
            }
            return builder.ToArray();
        }

        static async ValueTask<TResult[]> ToArrayAsync<TEnumerable, TEnumerator, TSource, TResult>(this TEnumerable source, AsyncSelectorAt<TSource, TResult> selector, CancellationToken cancellationToken)
            where TEnumerable : IAsyncValueEnumerable<TSource, TEnumerator>
            where TEnumerator : struct, IAsyncEnumerator<TSource>
        {
            using var builder = new LargeArrayBuilder<TResult>(initialize: true);
            var enumerator = source.GetAsyncEnumerator();
            await using (enumerator.ConfigureAwait(false))
            {
                checked
                {
                    for (var index = 0; await enumerator.MoveNextAsync().ConfigureAwait(false); index++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        builder.Add(await selector(enumerator.Current, index, cancellationToken).ConfigureAwait(false));
                    }
                }
            }
            return builder.ToArray();
        }

        static async ValueTask<TResult[]> ToArrayAsync<TEnumerable, TEnumerator, TSource, TResult>(this TEnumerable source, AsyncPredicate<TSource> predicate, AsyncSelector<TSource, TResult> selector, CancellationToken cancellationToken)
            where TEnumerable : IAsyncValueEnumerable<TSource, TEnumerator>
            where TEnumerator : struct, IAsyncEnumerator<TSource>
        {
            using var builder = new LargeArrayBuilder<TResult>(initialize: true);
            var enumerator = source.GetAsyncEnumerator();
            await using (enumerator.ConfigureAwait(false))
            {
                while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var item = enumerator.Current;
                    if (await predicate(item, cancellationToken).ConfigureAwait(false))
                        builder.Add(await selector(item, cancellationToken).ConfigureAwait(false));
                }
            }
            return builder.ToArray();
        }
    }
}