using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using NativeCollections.Allocators;
using NativeCollections.Utility;

namespace NativeCollections
{
    /// <summary>
    /// Represents a collection where values can be added/remove at the start or end.
    /// </summary>
    /// <typeparam name="T">Type of the elements.</typeparam>
    /// <seealso cref="NativeCollections.INativeContainer{T}" />
    /// <seealso cref="System.IDisposable" />
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeDequeDebugView<>))]
    unsafe public struct NativeDeque<T> : INativeContainer<T>, IDisposable where T : unmanaged
    {
        private T* _buffer;
        private int _capacity;
        private int _count;
        private int _head;
        private int _tail;

        private readonly int _allocatorID;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeDeque{T}"/> struct.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        public NativeDeque(int initialCapacity) : this(initialCapacity, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeDeque{T}"/> struct.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        /// <param name="allocator">The allocator.</param>
        /// <exception cref="ArgumentException">If the allocator is no in cache.
        /// </exception>
        public NativeDeque(int initialCapacity, Allocator allocator)
        {
            if (initialCapacity <= 0)
            {
                throw new ArgumentException("initialCapacity should be greater than 0.", nameof(initialCapacity));
            }

            if (Allocator.IsCached(allocator) is false)
            {
                throw new ArgumentException("Allocator is not in cache.", nameof(allocator));
            }

            _buffer = allocator.Allocate<T>(initialCapacity);
            _capacity = initialCapacity;
            _allocatorID = allocator.ID;
            _count = 0;
            _head = 0;
            _tail = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeDeque{T}"/> struct.
        /// </summary>
        /// <param name="elements">The initial elements.</param>
        public NativeDeque(Span<T> elements) : this(elements, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeDeque{T}"/> struct.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <param name="allocator">The allocator.</param>
        /// <exception cref="ArgumentException">If the allocator is no in cache.
        public NativeDeque(Span<T> elements, Allocator allocator)
        {
            if (Allocator.IsCached(allocator) is false)
            {
                throw new ArgumentException("Allocator is not in cache.", nameof(allocator));
            }

            if (elements.IsEmpty)
            {
                this = default;
            }
            else
            {
                int length = elements.Length;
                _buffer = allocator.Allocate<T>(length);

                void* source = Unsafe.AsPointer(ref elements.GetPinnableReference());
                Unsafe.CopyBlock(_buffer, source, (uint)(sizeof(T) * length));
                _capacity = length;
                _count = length;
                _allocatorID = allocator.ID;
                _head = 0;
                _tail = _capacity - 1;
            }
        }

        internal NativeDeque(void* pointer, int length, Allocator allocator)
        {
            Debug.Assert(pointer != null);
            Debug.Assert(length > 0);
            Debug.Assert(Allocator.IsCached(allocator));

            _buffer = (T*)pointer;
            _capacity = length;
            _count = length;
            _allocatorID = allocator.ID;
            _head = 0;
            _tail = length - 1;
        }

        private NativeDeque(ref NativeDeque<T> deque)
        {
            Debug.Assert(deque.IsValid);

            Allocator allocator = deque.GetAllocator()!;
            T* buffer = allocator.Allocate<T>(deque._capacity);
            Unsafe.CopyBlockUnaligned(buffer, deque._buffer, (uint)(sizeof(T) * deque._capacity));

            _buffer = buffer;
            _count = deque._count;
            _capacity = deque._capacity;
            _allocatorID = deque._allocatorID;
            _head = deque._head;
            _tail = deque._tail;
        }

        /// <summary>
        /// Gets the number of elements in the deque.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length => _count;

        /// <summary>
        /// Gets the number of elements this deque can hold before resize.
        /// </summary>
        /// <value>
        /// The capacity.
        /// </value>
        public int Capacity => _capacity;

        /// <summary>
        /// Gets a value indicating whether this deque have elements.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this deque is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => _count == 0;

        /// <summary>
        /// Checks if this deque is allocated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this deque is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid => _buffer != null;

        /// <summary>
        /// Gets the allocator.
        /// </summary>
        /// <returns></returns>
        public Allocator? GetAllocator()
        {
            return Allocator.GetAllocatorByID(_allocatorID);
        }

        /// <summary>
        /// Adds at element at the start of the deque.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void AddFirst(T value)
        {
            if (_buffer == null)
            {
                throw new ArgumentException("NativeDeque is invalid");
            }

            if (_count == _capacity)
            {
                Resize();
            }

            if(_count == 0)
            {
                _buffer[_head] = value;
            }
            else
            {
                _head = (_head - 1) < 0 ? _capacity - 1 : _head - 1;
                _buffer[_head] = value;
            }

            _count++;
        }

        /// <summary>
        /// Adds at element at the end of the deque.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void AddLast(T value)
        {
            if (_buffer == null)
            {
                throw new ArgumentException("NativeDeque is invalid");
            }

            if (_count == _capacity)
            {
                Resize();
            }

            if(_count == 0)
            {
                _buffer[_tail] = value;
            }
            else
            {
                _tail = (_tail + 1) % _capacity;
                _buffer[_tail] = value;
            }

            _count++;
        }

        /// <summary>
        /// Gets and removes the element at the start of the deque.
        /// </summary>
        /// <returns>The element at the start of de deque.</returns>
        /// <exception cref="InvalidOperationException">If the deque is empty.</exception>
        public T RemoveFirst()
        {
            if(!TryRemoveFirst(out T value))
            {
                throw new InvalidOperationException("NativeDeque is empty");
            }

            return value;
        }

        /// <summary>
        /// Gets and removes the element at the end of the deque.
        /// </summary>
        /// <returns>The element at the end of de deque.</returns>
        /// <exception cref="InvalidOperationException">If the deque is empty.</exception>
        public T RemoveLast()
        {
            if (!TryRemoveLast(out T value))
            {
                throw new InvalidOperationException("NativeDeque is empty");
            }

            return value;
        }

        /// <summary>
        /// Gets the element at the start of the deque.
        /// </summary>
        /// <returns>The element at the start of de deque.</returns>
        /// <exception cref="InvalidOperationException">If the deque is empty.</exception>
        public T PeekFirst()
        {
            if (!TryPeekFirst(out T value))
            {
                throw new InvalidOperationException("NativeDeque is empty");
            }

            return value;
        }

        /// <summary>
        /// Gets the element at the end of the deque.
        /// </summary>
        /// <returns>The element at the end of de deque.</returns>
        /// <exception cref="InvalidOperationException">If the deque is empty.</exception>
        public T PeekLast()
        {
            if (!TryPeekLast(out T value))
            {
                throw new InvalidOperationException("NativeDeque is empty");
            }

            return value;
        }

        /// <summary>
        /// Attemps to get and remove the element at the start of the deque.
        /// </summary>
        /// <param name="value">The value at the start of the deque.</param>
        /// <returns><c>true</c> if the value was removed.</returns>
        public bool TryRemoveFirst(out T value)
        {
            if (_buffer == null)
            {
                throw new ArgumentException("NativeDeque is invalid");
            }

            if (_count == 1)
            {
                value = _buffer[_head];
                _count--;
                return true;
            }

            if(_count > 0)
            {
                value = _buffer[_head];
                _buffer[_head] = default;
                _head = (_head + 1) % _capacity;
                _count--;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attemps to get and remove the element at the end of the deque.
        /// </summary>
        /// <param name="value">The value at the end of the deque.</param>
        /// <returns><c>true</c> if the value was removed.</returns>
        public bool TryRemoveLast(out T value)
        {
            if (_buffer == null)
            {
                throw new ArgumentException("NativeDeque is invalid");
            }

            if (_count == 1)
            {
                value = _buffer[_tail];
                _count--;
                return true;
            }

            if (_count > 0)
            {
                value = _buffer[_tail];
                _buffer[_tail] = default;
                _tail = _tail - 1 < 0 ? _capacity - 1 : _tail - 1;
                _count--;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attemps to get the element at the start of the deque.
        /// </summary>
        /// <param name="value">The value at the start of the deque.</param>
        /// <returns><c>true</c> if the value exists.</returns>
        public bool TryPeekFirst(out T value)
        {
            if (_buffer == null)
            {
                throw new ArgumentException("NativeDeque is invalid");
            }

            if (_count > 0)
            {
                value = _buffer[_head];
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attemps to get the element at the end of the deque.
        /// </summary>
        /// <param name="value">The value at the end of the deque.</param>
        /// <returns><c>true</c> if the value exists.</returns>
        public bool TryPeekLast(out T value)
        {
            if (_buffer == null)
            {
                throw new ArgumentException("NativeDeque is invalid");
            }

            if (_count > 0)
            {
                value = _buffer[_tail];
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Removes the contents of this deque.
        /// </summary>
        public void Clear()
        {
            if (_buffer == null)
            {
                throw new ArgumentException("NativeDeque is invalid");
            }

            if (_count == 0)
                return;

            _count = 0;
            _head = 0;
            _tail = 0;
        }

        /// <summary>
        /// Determines whether this deque contains the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the deque contains the value; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T value)
        {
            if (_buffer == null)
            {
                throw new ArgumentException("NativeDeque is invalid");
            }

            if (_count == 0)
            {
                return false;
            }

            var comparer = EqualityComparer<T>.Default;

            for(int i = 0; i < _count; i++)
            {
                int next = (_head + i) % _capacity;
                if(comparer.Equals(_buffer[next], value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Reverses the order of the elements of this instance.
        /// </summary>
        public void Reverse()
        {
            if (_buffer == null)
            {
                throw new ArgumentException("NativeDeque is invalid");
            }

            if (_count == 0)
                return;

            int head = _head;
            int tail = _tail;

            int count = _count / 2;
            while(count > 0)
            {
                T value = _buffer[head];
                _buffer[head] = _buffer[tail];
                _buffer[tail] = value;

                head = (head + 1) % _capacity;
                tail = (tail - 1) < 0 ? _capacity - 1 : tail - 1;
                --count;
            }
        }

        /// <summary>
        /// Removes the excess space in this deque.
        /// </summary>
        public void TrimExcess()
        {
            TrimExcess(_count);
        }

        /// <summary>
        /// Removes the excess space in this deque.
        /// </summary>
        /// <param name="capacity">The min capacity.</param>
        public void TrimExcess(int capacity)
        {
            if (_buffer == null)
            {
                throw new ArgumentException("NativeDeque is invalid");
            }

            if (capacity < _count)
            {
                return;
            }

            Resize(capacity);
        }

        /// <summary>
        /// Copies the content of this deque to a <see cref="Span{T}" />.
        /// </summary>
        /// <param name="span">The destination span to copy the data.</param>
        /// <param name="destinationIndex">Start index of the destination where start to copy.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentException">Span is empty</exception>
        /// <exception cref="InvalidOperationException">Stack is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the range provided by the destination index and count is invalid.</exception>
        public void CopyTo(in Span<T> span, int destinationIndex, int count)
        {
            if (span.IsEmpty)
                throw new ArgumentException("Span is empty");

            if (_buffer == null)
                throw new InvalidOperationException("NativeDeque is invalid");

            if (destinationIndex < 0 || destinationIndex > span.Length)
                throw new ArgumentOutOfRangeException(nameof(destinationIndex), destinationIndex.ToString());

            if (count < 0 || count > _count || count > (span.Length - destinationIndex))
                throw new ArgumentOutOfRangeException(nameof(count), count.ToString());

            int i = 0;
            int pos = destinationIndex;
            do
            {
                int next = (_head + i) % _capacity;
                span[pos] = _buffer[next];
                ++pos;
                ++i;
            }
            while (i < count);
        }

        /// <summary>
        /// Ensures this deque can hold the specified amount of values before resize.
        /// </summary>
        /// <param name="capacity">The min capacity.</param>
        public void EnsureCapcity(int capacity)
        {
            if (_buffer == null)
            {
                throw new ArgumentException("NativeDeque is invalid");
            }

            if (capacity > _capacity)
            {
                Resize(capacity);
            }
        }

        /// <summary>
        /// Allocates an array with the elements of this deque.
        /// </summary>
        /// <returns>An newly allocated array with the elements of this instance.</returns>
        public T[] ToArray()
        {
            if (_buffer == null)
            {
                throw new ArgumentException("NativeDeque is invalid");
            }

            if (_count == 0)
            {
                return Array.Empty<T>();
            }

            T[] array = new T[_count];
            CopyTo(array, 0, _count);
            return array;
        }

        /// <summary>
        /// Creates a new <see cref="NativeArray{T}"/> with the elements of this deque.
        /// </summary>
        /// <returns>A new array with the elements of this instance.</returns>
        public NativeArray<T> ToNativeArray()
        {
            if (_buffer == null)
            {
                throw new ArgumentException("NativeDeque is invalid");
            }

            if (_count == 0)
                return default;

            NativeArray<T> array = new NativeArray<T>(_count, GetAllocator()!);
            int i = 0;
            do
            {
                int next = (_head + i) % _capacity;
                array[i] = _buffer[next];
                ++i;
            }
            while (i < _count);
            return array;
        }

        /// <summary>
        /// Creates a new <see cref="NativeArray{T}"/> with the elements of this deque and dispose this deque.
        /// </summary>
        /// <param name="createNewArrayIfNeeded">If <c>true</c> a new array will be created if the capacity of this
        /// queue is different than its length; otherwise is guaranteed the new array will use this stack memory.</param>
        /// <returns>A newly created array with this list elements.</returns>
        public NativeArray<T> ToNativeArrayAndDispose(bool createNewArrayIfNeeded = true)
        {
            if (_buffer == null)
            {
                throw new ArgumentException("NativeDeque is invalid");
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
        /// Releases the allocated resources of this deque.
        /// </summary>
        public void Dispose()
        {
            if (_buffer == null)
            {
                return;
            }

            if (Allocator.IsCached(_allocatorID))
            {
                GetAllocator()!.Free(_buffer);
                this = default;
            }
        }

        /// <summary>
        /// Gets a string representation of the elements of this deque.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this deque elements.
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Resize()
        {
            Resize(_capacity * 2);
        }

        private void Resize(int capacity)
        {
            Debug.Assert(_buffer != null);
            T* newBuffer = GetAllocator()!.Allocate<T>(capacity);

            int i = 0;
            do
            {
                int next = (_head + i) % _capacity;
                newBuffer[i] = _buffer[next];
                ++i;
            }
            while (i < _count);

            GetAllocator()!.Free(_buffer);
            _buffer = newBuffer;
            _capacity = capacity;
            _head = 0;
            _tail = _count - 1;
        }
        
        /// <summary>
        /// Gets a deep clone of this instance.
        /// </summary>
        /// <returns>A copy of this instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeDeque<T> Clone()
        {
            return _buffer == null? throw new ArgumentException("NativeDeque is invalid"): new NativeDeque<T>(ref this);
        }

        /// <summary>
        /// Gets an enumerator over the elements of the deque.
        /// </summary>
        /// <returns>An enumerator over this deque elements.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator()
        {
            return _buffer == null? default : new Enumerator(ref this);
        }

        /// <summary>
        /// An enumerator over the elements of a <see cref="NativeDeque{T}"/>.
        /// </summary>
        public ref struct Enumerator
        {
            private readonly void* _data;
            private readonly int _count;
            private readonly int _capacity;
            private readonly int _head;
            private int _index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(ref NativeDeque<T> queue)
            {
                _data = queue._buffer;
                _capacity = queue._capacity;
                _count = queue._count;
                _head = queue._head;
                _index = -1;
            }

            /// <summary>
            /// Gets a reference to the current value.
            /// </summary>
            public ref T Current
            {
                get
                {
                    if (_index < 0 || _index > _count)
                        throw new ArgumentOutOfRangeException("index", _index.ToString());

                    ref T startAddress = ref Unsafe.AsRef<T>(_data);
                    int next = (_index + _head) % _capacity;
                    return ref Unsafe.Add(ref startAddress, next);
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
                if (_count == 0)
                    return false;

                int i = _index + 1;

                if (i < _count)
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
