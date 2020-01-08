using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NativeCollections.Allocators;
using NativeCollections.Utility;

namespace NativeCollections
{
    /// <summary>
    /// Represents a collection FIFO (first-in first-out) where the first element added is the first to be removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="NativeCollections.INativeContainer{T}" />
    /// <seealso cref="System.IDisposable" />
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeQueueDebugView<>))]
    unsafe public struct NativeQueue<T> : INativeContainer<T>, IDisposable where T : unmanaged
    {
        internal T* _buffer;
        private int _capacity;
        private int _count;
        private int _head;
        private int _tail;

        private readonly int _allocatorID;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeQueue{T}"/> struct.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        public NativeQueue(int initialCapacity) : this(initialCapacity, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeQueue{T}"/> struct.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        /// <param name="allocator">The allocator.</param>
        /// <exception cref="ArgumentException">capacity must be greater than 0: {initialCapacity}</exception>
        public NativeQueue(int initialCapacity, Allocator allocator)
        {
            if (initialCapacity <= 0)
            {
                throw new ArgumentException($"capacity must be greater than 0: {initialCapacity}");
            }

            if (allocator.ID <= 0)
            {
                throw new ArgumentException("Allocator is not in cache.", "allocator");
            }

            _buffer = (T*)allocator.Allocate(initialCapacity, sizeof(T));
            _capacity = initialCapacity;
            _count = _head = _tail = 0;
            _allocatorID = allocator.ID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeQueue{T}"/> struct.
        /// </summary>
        /// <param name="elements">The initial elements.</param>
        public NativeQueue(Span<int> elements) : this(elements, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeQueue{T}"/> struct.
        /// </summary>
        /// <param name="elements">The initial elements.</param>
        /// <param name="allocator">The allocator.</param>
        public NativeQueue(Span<int> elements, Allocator allocator)
        {
            if (allocator.ID <= 0)
            {
                throw new ArgumentException("Allocator is not in cache.", "allocator");
            }

            if (elements.IsEmpty)
            {
                this = default;
            }
            else
            {
                _buffer = (T*)allocator.Allocate(elements.Length, sizeof(T));
                _capacity = elements.Length;
                _count = _capacity;
                _head = 0;
                _tail = 0;
                _allocatorID = allocator.ID;

                void* source = Unsafe.AsPointer(ref MemoryMarshal.GetReference(elements));
                Unsafe.CopyBlock(_buffer, source, (uint)(sizeof(T) * _capacity));
            }
        }

        private NativeQueue(ref NativeQueue<T> queue)
        {
            if (!queue.IsValid)
            {
                throw new ArgumentException("queue is invalid");
            }

            Allocator allocator = queue.GetAllocator()!;
            T* buffer = allocator.Allocate<T>(queue._capacity);
            Unsafe.CopyBlockUnaligned(buffer, queue._buffer, (uint)(sizeof(T) * queue._capacity));

            _buffer = buffer;
            _capacity = queue._capacity;
            _count = queue._count;
            _allocatorID = queue._allocatorID;
            _head = queue._head;
            _tail = queue._tail;
        }

        /// <summary>
        /// Gets the number of elements in the queue.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length => _count;

        /// <summary>
        /// Gets the number of elements this queue can hold before resize.
        /// </summary>
        /// <value>
        /// The capacity.
        /// </value>
        public int Capacity => _capacity;

        /// <summary>
        /// Checks if this queue is allocated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid => _buffer != null;

        /// <summary>
        /// Gets a value indicating whether this queue have elements.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => _count == 0;

        /// <summary>
        /// Gets the allocator.
        /// </summary>
        /// <returns></returns>
        public Allocator? GetAllocator()
        {
            return Allocator.GetAllocatorByID(_allocatorID);
        }

        /// <summary>
        /// Adds the specified element at the start of the queue.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Enqueue(T value)
        {
            if (_count == _capacity)
            {
                RequireCapacity(_count + 1);
            }

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            Unsafe.Add(ref startAddress, _tail) = value;
            _tail = (_tail + 1) % _capacity;
            _count++;
        }

        /// <summary>
        /// Gets and removes the first element in the queue.
        /// </summary>
        /// <returns>The first element of the queue.</returns>
        /// <exception cref="InvalidOperationException">If the Queue is empty</exception>
        public T Dequeue()
        {
            if (_count == 0)
                throw new InvalidOperationException("The queue is empty");

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            T value = Unsafe.Add(ref startAddress, _head);
            Unsafe.Add(ref startAddress, _head) = default;
            _head = (_head + 1) % _capacity;
            _count--;
            return value;
        }

        /// <summary>
        /// Gets the first element in the queue.
        /// </summary>
        /// <returns>The first element of the queue.</returns>
        /// <exception cref="InvalidOperationException">If the Queue is empty</exception>
        public T Peek()
        {
            if (_count == 0)
                throw new InvalidOperationException("The queue is empty");

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            T value = Unsafe.Add(ref startAddress, _head);
            return value;
        }

        /// <summary>
        /// Attemps to get and removes the first element in the queue.
        /// </summary>
        /// <param name="value">The first value on the queue.</param>
        /// <returns><c>true</c> If the first element was removed.</returns>
        public bool TryDequeue(out T value)
        {
            if (_count == 0)
            {
                value = default;
                return false;
            }
            else
            {
                ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
                value = Unsafe.Add(ref startAddress, _head);
                Unsafe.Add(ref startAddress, _head) = default;
                _head = (_head + 1) % _capacity;
                _count--;
                return true;
            }
        }

        /// <summary>
        /// Attemps to get the first element in the queue.
        /// </summary>
        /// <param name="value">The first value on the queue.</param>
        /// <returns><c>true</c> If the first element exists.</returns>
        public bool TryPeek(out T value)
        {
            if (_count == 0)
            {
                value = default;
                return false;
            }
            else
            {
                ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
                value = Unsafe.Add(ref startAddress, _head);
                return true;
            }
        }

        /// <summary>
        /// Clears the content of this queue.
        /// </summary>
        public void Clear()
        {
            if(_count == 0)
                return;

            Unsafe.InitBlockUnaligned(_buffer, 0, (uint)(sizeof(T) * _capacity));
            _head = 0;
            _tail = 0;
            _count = 0;
        }

        /// <summary>
        /// Determines whether this queue contains the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the queue contains the specified value; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T value)
        {
            if(_count == 0)
            {
                return false;
            }

            var comparer = EqualityComparer<T>.Default;

            int i = _head;
            do
            {
                if (comparer.Equals(_buffer[i], value))
                {
                    return true;
                }

                i = (i + 1) % _capacity;

                if (i == _tail)
                {
                    break;
                }
            }
            while (true);

            return false;
        }

        /// <summary>
        /// Removes the excess spaces in the queue.
        /// </summary>
        public void TrimExcess()
        {
            TrimExcess(_count);
        }

        /// <summary>
        /// Removes the excess spaces in the queue.
        /// </summary>
        /// <param name="capacity">The min capacity.</param>
        public void TrimExcess(int capacity)
        {
            if (capacity >= _count)
            {
                Resize(capacity);
            }
        }

        /// <summary>
        /// Allocates an array with the elements of this queue.
        /// </summary>
        /// <returns>An newly allocated array with the elements of this instance.</returns>
        public T[] ToArray()
        {
            if (_count == 0)
            {
                return Array.Empty<T>();
            }

            T[] array = new T[_count];
            CopyTo(array, 0, _count);
            return array;
        }

        /// <summary>
        /// Creates a new <see cref="NativeArray{T}"/> with the elements of this queue.
        /// </summary>
        /// <returns>A new array with the elements of this instance.</returns>
        public NativeArray<T> ToNativeArray()
        {
            if (_count == 0)
                return default;

            NativeArray<T> array = new NativeArray<T>(_count, GetAllocator()!);
            int i = 0;
            int next = _head;

            do
            {
                array[i] = _buffer[next];
                ++i;

                next = (next + 1) % _capacity;
                if (next == _tail)
                {
                    break;
                }
            }
            while (true);
            return array;
        }

        /// <summary>
        /// Creates a new <see cref="NativeArray{T}"/> with the elements of this queue and dispose this queue.
        /// </summary>
        /// <param name="createNewArrayIfNeeded">If <c>true</c> a new array will be created if the capacity of this
        /// queue is different than its length; otherwise is guaranteed the new array will use this stack memory.</param>
        /// <returns>A newly created array with this list elements.</returns>
        public NativeArray<T> ToNativeArrayAndDispose(bool createNewArrayIfNeeded = true)
        {
            if (_buffer == null)
            {
                return default;
            }

            if (_count == _capacity || !createNewArrayIfNeeded)
            {
                // NativeArray will owns this instance memory
                NativeArray<T> array = new NativeArray<T>(_buffer, _capacity, GetAllocator()!);

                // Not actual dispose, just invalidate this instance
                this = default;
                return array;
            }
            else
            {
                NativeArray<T> array = ToNativeArray();
                Dispose();
                return array;
            }
        }

        /// <summary>
        /// Copies the content of this queue to a <see cref="Span{T}" />.
        /// </summary>
        /// <param name="span">The destination span to copy the data.</param>
        /// <param name="destinationIndex">Start index of the destination where start to copy.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentException">Span is empty</exception>
        /// <exception cref="InvalidOperationException">Queue is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the range provided by the destination index and count is invalid.</exception>
        public void CopyTo(in Span<T> span, int destinationIndex, int count)
        {
            if (span.IsEmpty)
                throw new ArgumentException("Span is empty");

            if (_buffer == null)
                throw new InvalidOperationException("NativeQueue is invalid");

            if (destinationIndex < 0 || destinationIndex > span.Length)
                throw new ArgumentOutOfRangeException(nameof(destinationIndex), destinationIndex.ToString());

            if (count < 0 || count > _count || count > (span.Length - destinationIndex))
                throw new ArgumentOutOfRangeException(nameof(count), count.ToString());

            int i = destinationIndex;
            int next = _head;

            do
            {
                span[i] = _buffer[next];
                ++i;

                next = (next + 1) % _capacity;
                if (--count == 0)
                {
                    break;
                }
            }
            while (true);
        }

        /// <summary>
        /// Ensures this queue have enough capacity to hold the specified number of elements.
        /// </summary>
        /// <param name="capacity">The min capacity.</param>
        public void EnsureCapacity(int capacity)
        {
            if (capacity > _capacity)
            {
                Resize(capacity);
            }
        }

        private void RequireCapacity(int min)
        {
            if (min > _capacity)
            {
                if (min < _capacity * 2)
                {
                    Resize(_capacity * 2);
                }
                else
                {
                    Resize(min);
                }
            }
        }

        private void Resize(int newCapacity)
        {
            if (_buffer == null)
                return;

            T* newBuffer = (T*)GetAllocator()!.Allocate<T>(newCapacity);
            ref T destination = ref Unsafe.AsRef<T>(newBuffer);
            ref T source = ref Unsafe.AsRef<T>(_buffer);

            int next = _head;
            int i = 0;
            do
            {
                Unsafe.Add(ref destination, i) = Unsafe.Add(ref source, next);
                ++i;

                next = (next + 1) % _capacity;
                if (next == _tail)
                {
                    break;
                }
            }
            while (true);

            GetAllocator()!.Free(_buffer);
            _buffer = newBuffer;
            _capacity = newCapacity;
            _tail = _count;
            _head = 0;
        }

        /// <summary>
        /// Releases the resources used for this queue.
        /// </summary>
        public void Dispose()
        {
            if (_buffer == null)
                return;

            if (Allocator.IsCached(_allocatorID))
            {
                GetAllocator()!.Free(_buffer);
                this = default;
            }
        }

        /// <summary>
        /// Gets a string representation of the elements of this queue.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this queue elements.
        /// </returns>
        public override string ToString()
        {
            if (_count == 0)
            {
                return "[]";
            }

            StringBuilder sb = StringBuilderCache.Acquire();
            sb.Append('[');

            var enumerator = GetEnumerator();

            if (enumerator.MoveNext())
            {
                while (true)
                {
                    sb.Append(enumerator.Current.ToString());

                    if (enumerator.MoveNext())
                    {
                        sb.Append(", ");
                    }
                    else
                    {
                        break;
                    }
                }
            }            

            sb.Append(']');
            return StringBuilderCache.ToStringAndRelease(ref sb!);
        }

        /// <summary>
        /// Gets a deep clone of this instance.
        /// </summary>
        /// <returns>A copy of this instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeQueue<T> Clone()
        {
            if (_buffer == null)
            {
                return default;
            }

            return new NativeQueue<T>(ref this);
        }

        /// <summary>
        /// Gets an enumerator over the elements of the queue.
        /// </summary>
        /// <returns>An enumerator over this queue elements.</returns>
        public Enumerator GetEnumerator()
        {
            Debug.Assert(_buffer != null);

            if (_buffer == null)
            {
                return default;
            }

            return new Enumerator(ref this);
        }

        /// <summary>
        /// An enumerator over the elements of a <see cref="NativeQueue{T}"/>.
        /// </summary>
        public ref struct Enumerator
        {
            private readonly void* _data;
            private readonly int _length;
            private readonly int _head;
            private readonly int _tail;
            private int _index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(ref NativeQueue<T> queue)
            {
                _data = queue._buffer;
                _length = queue._capacity;
                _head = queue._head;
                _tail = queue._tail;
                _index = -1;
            }

            /// <summary>
            /// Gets a reference to the current value.
            /// </summary>
            public ref T Current
            {
                get
                {
                    if (_index < 0 || _index > _length)
                        throw new ArgumentOutOfRangeException("index", _index.ToString());

                    ref T startAddress = ref Unsafe.AsRef<T>(_data);
                    return ref Unsafe.Add(ref startAddress, _index);
                }
            }

            /// <summary>
            /// Invalidates this enumerator.
            /// </summary>
            public void Dispose()
            {
                this = default;
            }

            /// <summary>
            /// Moves to the next value.
            /// </summary>
            /// <returns><c>true</c> if moved.</returns>
            public bool MoveNext()
            {
                if (_length == 0)
                    return false;

                if (_index == -1)
                {
                    _index = _head;
                    return true;
                }

                int i = (_index + 1) % _length;
                if (i != _tail)
                {
                    _index = i;
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Resets this enumerator.
            /// </summary>
            public void Reset()
            {
                _index = -1;
            }
        }
    }
}
