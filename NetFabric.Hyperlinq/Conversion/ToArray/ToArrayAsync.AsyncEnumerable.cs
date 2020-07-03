using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFabric.Hyperlinq
{
    public static partial class AsyncEnumerableExtensions
    {
        
        static async ValueTask<TSource[]> ToArrayAsync<TSource>(IAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
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
    }
}