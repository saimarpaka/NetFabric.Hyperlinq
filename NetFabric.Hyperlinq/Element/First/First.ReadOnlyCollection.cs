using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NetFabric.Hyperlinq
{
    public static partial class ReadOnlyCollection
    {
        public static TSource First<TEnumerable, TEnumerator, TSource>(this TEnumerable source) 
            where TEnumerable : IReadOnlyCollection<TSource>
            where TEnumerator : IEnumerator<TSource>
        {
            if (source.TryFirst<TEnumerable, TEnumerator, TSource>(out var value))
                return value;
                
            return ThrowHelper.ThrowEmptySequence<TSource>();
        }

        public static TSource FirstOrDefault<TEnumerable, TEnumerator, TSource>(this TEnumerable source) 
            where TEnumerable : IReadOnlyCollection<TSource>
            where TEnumerator : IEnumerator<TSource>
        {
            if (source.TryFirst<TEnumerable, TEnumerator, TSource>(out var value))
                return value;
                
            return default;
        }

        public static TSource? FirstOrNull<TEnumerable, TEnumerator, TSource>(this TEnumerable source) 
            where TEnumerable : IReadOnlyCollection<TSource>
            where TEnumerator : IEnumerator<TSource>
            where TSource : struct
        {
            if (source.TryFirst<TEnumerable, TEnumerator, TSource>(out var value))
                return value;
                
            return null;
        }

        public static TSource First<TEnumerable, TEnumerator, TSource>(this TEnumerable source, Func<TSource, bool> predicate) 
            where TEnumerable : IReadOnlyCollection<TSource>
            where TEnumerator : IEnumerator<TSource>
        {
            if (predicate is null) ThrowHelper.ThrowArgumentNullException(nameof(predicate));

            if (source.TryFirst<TEnumerable, TEnumerator, TSource>(predicate, out var value))
                return value;
                
            return ThrowHelper.ThrowEmptySequence<TSource>();
        }

        public static TSource FirstOrDefault<TEnumerable, TEnumerator, TSource>(this TEnumerable source, Func<TSource, bool> predicate) 
            where TEnumerable : IReadOnlyCollection<TSource>
            where TEnumerator : IEnumerator<TSource>
        {
            if (predicate is null) ThrowHelper.ThrowArgumentNullException(nameof(predicate));

            if (source.TryFirst<TEnumerable, TEnumerator, TSource>(predicate, out var value))
                return value;
                
            return default;
        }

        public static TSource? FirstOrNull<TEnumerable, TEnumerator, TSource>(this TEnumerable source, Func<TSource, bool> predicate) 
            where TEnumerable : IReadOnlyCollection<TSource>
            where TEnumerator : IEnumerator<TSource>
            where TSource : struct
        {
            if (predicate is null) ThrowHelper.ThrowArgumentNullException(nameof(predicate));

            if (source.TryFirst<TEnumerable, TEnumerator, TSource>(predicate, out var value))
                return value;
                
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool TryFirst<TEnumerable, TEnumerator, TSource>(this TEnumerable source, out TSource value) 
            where TEnumerable : IReadOnlyCollection<TSource>
            where TEnumerator : IEnumerator<TSource>
        {
            if (source.Count != 0) 
            {
                using (var enumerator = (TEnumerator)source.GetEnumerator())
                {
                    value = enumerator.Current;
                    return enumerator.MoveNext();
                }    
            } 

            value = default;
            return false;            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool TryFirst<TEnumerable, TEnumerator, TSource>(this TEnumerable source, Func<TSource, bool> predicate, out TSource value) 
            where TEnumerable : IReadOnlyCollection<TSource>
            where TEnumerator : IEnumerator<TSource>
        {
            if (source.Count != 0) 
            {
                using (var enumerator = (TEnumerator)source.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (predicate(enumerator.Current))
                        {
                            value = enumerator.Current;
                            return true;
                        }
                    }
                }
            }        

            value = default;
            return false;
        }    
    }
}