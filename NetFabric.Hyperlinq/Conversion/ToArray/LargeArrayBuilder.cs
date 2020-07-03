// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// based on https://github.com/dotnet/runtime/blob/master/src/libraries/Common/src/System/Collections/Generic/LargeArrayBuilder.SpeedOpt.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NetFabric.Hyperlinq
{
    /// <summary>
    /// Helper type for building dynamically-sized arrays while minimizing allocations and copying.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    struct LargeArrayBuilder<T> : IDisposable
    {
        const int StartingCapacity = 4;
        const int ResizeLimit = 8;

        readonly int _maxCapacity;  // The maximum capacity this builder can have.
        T[] _first;                 // The first buffer we store items in. Resized until ResizeLimit.
        ArrayBuilder<T[]> _buffers; // After ResizeLimit * 2, we store previous buffers we've filled out here.
        T[] _current;               // Current buffer we're reading into. If _count <= ResizeLimit, this is _first.
        int _index;                 // Index into the current buffer.

        /// <summary>
        /// Constructs a new builder.
        /// </summary>
        /// <param name="initialize">Pass <c>true</c>.</param>
        public LargeArrayBuilder(bool initialize)
            : this(maxCapacity: int.MaxValue)
        {
            // This is a workaround for C# not having parameterless struct constructors yet.
            // Once it gets them, replace this with a parameterless constructor.
            Debug.Assert(initialize);
        }

        /// <summary>
        /// Constructs a new builder with the specified maximum capacity.
        /// </summary>
        /// <param name="maxCapacity">The maximum capacity this builder can have.</param>
        /// <remarks>
        /// Do not add more than <paramref name="maxCapacity"/> items to this builder.
        /// </remarks>
        public LargeArrayBuilder(int maxCapacity)
            : this()
        {
            Debug.Assert(maxCapacity >= 0);

            _buffers = new ArrayBuilder<T[]>(0);
            _first = _current = Array.Empty<T>();
            _maxCapacity = maxCapacity;
        }

        /// <summary>
        /// Gets the number of items added to the builder.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Adds an item to this builder.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <remarks>
        /// Use <see cref="Add"/> if adding to the builder is a bottleneck for your use case.
        /// Otherwise, use <see cref="SlowAdd"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add([AllowNull] T item)
        {
            Debug.Assert(_maxCapacity > Count);

            var index = _index;
            var current = _current;

            // Must be >= and not == to enable range check elimination
            if ((uint)index >= (uint)current.Length)
            {
                AddWithBufferAllocation(item);
            }
            else
            {
                current[index] = item;
                _index = index + 1;
            }

            Count++;
        }

        // Non-inline to improve code quality as uncommon path
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddWithBufferAllocation([AllowNull] T item)
        {
            AllocateBuffer();
            _current[_index++] = item;
        }

        /// <summary>
        /// Adds a range of items to this builder.
        /// </summary>
        /// <param name="items">The sequence to add.</param>
        /// <remarks>
        /// It is the caller's responsibility to ensure that adding <paramref name="items"/>
        /// does not cause the builder to exceed its maximum capacity.
        /// </remarks>
        public void AddRange(IEnumerable<T> items)
        {
            Debug.Assert(items != null);

            using var enumerator = items.GetEnumerator();
            var destination = _current;
            var index = _index;

            // Continuously read in items from the enumerator, updating _count
            // and _index when we run out of space.

            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;

                if ((uint)index >= (uint)destination.Length)
                {
                    AddWithBufferAllocation(item, ref destination, ref index);
                }
                else
                {
                    destination[index] = item;
                }

                index++;
            }

            // Final update to _count and _index.
            Count += index - _index;
            _index = index;
        }

        // Non-inline to improve code quality as uncommon path
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddWithBufferAllocation([AllowNull] T item, ref T[] destination, ref int index)
        {
            Count += index - _index;
            _index = index;
            AllocateBuffer();
            destination = _current;
            index = _index;
            _current[index] = item!;
        }

        /// <summary>
        /// Copies the contents of this builder to the specified array.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="arrayIndex">The index in <paramref name="array"/> to start copying to.</param>
        /// <param name="count">The number of items to copy.</param>
        public void CopyTo(T[] array, int arrayIndex, int count)
        {
            Debug.Assert(arrayIndex >= 0);
            Debug.Assert(count >= 0 && count <= Count);
            Debug.Assert(array.Length - arrayIndex >= count);

            for (int i = 0; count > 0; i++)
            {
                // Find the buffer we're copying from.
                T[] buffer = GetBuffer(index: i);

                // Copy until we satisfy count, or we reach the end of the buffer.
                int toCopy = Math.Min(count, buffer.Length);
                Array.Copy(buffer, 0, array, arrayIndex, toCopy);

                // Increment variables to that position.
                count -= toCopy;
                arrayIndex += toCopy;
            }
        }

        /// <summary>
        /// Retrieves the buffer at the specified index.
        /// </summary>
        /// <param name="index">The index of the buffer.</param>
        public T[] GetBuffer(int index)
        {
            Debug.Assert(index >= 0 && index < _buffers.Count + 2);

            return index == 0 
                ? _first 
                : index <= _buffers.Count 
                    ? _buffers[index - 1] 
                    : _current;
        }

        /// <summary>
        /// Adds an item to this builder.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <remarks>
        /// Use <see cref="Add"/> if adding to the builder is a bottleneck for your use case.
        /// Otherwise, use <see cref="SlowAdd"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void SlowAdd([AllowNull] T item) => Add(item);

        /// <summary>
        /// Creates an array from the contents of this builder.
        /// </summary>
        public T[] ToArray()
        {
            if (TryMove(out var array))
            {
                // No resizing to do.
                return array;
            }

            array = new T[Count];
            CopyTo(array, 0, Count);
            return array;
        }

        /// <summary>
        /// Attempts to transfer this builder into an array without copying.
        /// </summary>
        /// <param name="array">The transferred array, if the operation succeeded.</param>
        /// <returns><c>true</c> if the operation succeeded; otherwise, <c>false</c>.</returns>
        public bool TryMove(out T[] array)
        {
            array = _first;
            return Count == _first.Length;
        }

        void AllocateBuffer()
        {
            // - On the first few adds, simply resize _first.
            // - When we pass ResizeLimit, allocate ResizeLimit elements for _current
            //   and start reading into _current. Set _index to 0.
            // - When _current runs out of space, add it to _buffers and repeat the
            //   above step, except with _current.Length * 2.
            // - Make sure we never pass _maxCapacity in all of the above steps.

            Debug.Assert((uint)_maxCapacity > (uint)Count);
            Debug.Assert(_index == _current.Length, $"{nameof(AllocateBuffer)} was called, but there's more space.");

            // If _count is int.MinValue, we want to go down the other path which will raise an exception.
            if ((uint)Count < (uint)ResizeLimit)
            {
                // We haven't passed ResizeLimit. Resize _first, copying over the previous items.
                Debug.Assert(_current == _first && Count == _first.Length);

                int nextCapacity = Math.Min(Count == 0 ? StartingCapacity : Count * 2, _maxCapacity);

                _current = new T[nextCapacity];
                Array.Copy(_first, _current, Count);
                _first = _current;
            }
            else
            {
                Debug.Assert(_maxCapacity > ResizeLimit);
                Debug.Assert(Count == ResizeLimit ^ _current != _first);

                int nextCapacity;
                if (Count == ResizeLimit)
                {
                    nextCapacity = ResizeLimit;
                }
                else
                {
                    // Example scenario: Let's say _count == 64.
                    // Then our buffers look like this: | 8 | 8 | 16 | 32 |
                    // As you can see, our count will be just double the last buffer.
                    // Now, say _maxCapacity is 100. We will find the right amount to allocate by
                    // doing min(64, 100 - 64). The lhs represents double the last buffer,
                    // the rhs the limit minus the amount we've already allocated.

                    Debug.Assert(Count >= ResizeLimit * 2);
                    Debug.Assert(Count == _current.Length * 2);

                    _buffers.Add(_current);
                    nextCapacity = Math.Min(Count, _maxCapacity - Count);
                }

                _current = new T[nextCapacity];
                _index = 0;
            }
        }

        public void Dispose() => _buffers.Dispose();
    }
}