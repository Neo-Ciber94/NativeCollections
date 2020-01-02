using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using NativeCollections.Allocators;
using NativeCollections.Utility;

namespace NativeCollections
{
    unsafe public struct NativeDeque<T> : INativeContainer<T>, IDisposable where T : unmanaged
    {
        private T* _buffer;
        private int _capacity;
        private int _count;
        private int _allocatorID;

        private int _head;
        private int _tail;

        public NativeDeque(int initialCapacity) : this(initialCapacity, Allocator.Default) { }

        public NativeDeque(int initialCapacity, Allocator allocator)
        {
            if (initialCapacity <= 0)
                throw new ArgumentException("initialCapacity should be greater than 0.", nameof(initialCapacity));

            _buffer = allocator.Allocate<T>(initialCapacity);
            _capacity = initialCapacity;
            _allocatorID = allocator.ID;
            _count = 0;
            _head = 0;
            _tail = 0;
        }

        public NativeDeque(Span<T> elements) : this(elements, Allocator.Default) { }

        public NativeDeque(Span<T> elements, Allocator allocator)
        {
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
                _tail = length;
            }
        }

        public int Length => _count;

        public int Capacity => _capacity;

        public bool IsEmpty => _count == 0;

        public bool IsValid => _buffer != null;

        public Allocator? GetAllocator()
        {
            return Allocator.GetAllocatorByID(_allocatorID);
        }

        public void AddFirst(T value)
        {
            if(_count == _capacity)
            {
                Resize();
            }

            _buffer[_head] = value;
            _head = (_head - 1) < 0 ? _capacity : _head - 1;
            _count++;
        }

        public void AddLast(T value)
        {
            if (_count == _capacity)
            {
                Resize();
            }

            _tail = (_tail + 1) % _capacity;
            _buffer[_tail] = value;
            _count++;
        }

        public T PopFirst()
        {
            if(!TryPopFirst(out T value))
            {
                throw new InvalidOperationException("NativeDeque is empty");
            }

            return value;
        }

        public T PopLast()
        {
            if (!TryPopLast(out T value))
            {
                throw new InvalidOperationException("NativeDeque is empty");
            }

            return value;
        }

        public T PeekFirst()
        {
            if (!TryPeekFirst(out T value))
            {
                throw new InvalidOperationException("NativeDeque is empty");
            }

            return value;
        }

        public T PeekLast()
        {
            if (!TryPopLast(out T value))
            {
                throw new InvalidOperationException("NativeDeque is empty");
            }

            return value;
        }

        public bool TryPopFirst(out T value)
        {
            if(_count > 0)
            {
                int next = (_head + 1) % _capacity;
                value = _buffer[next];
                _head = next;
                _count--;
                return true;
            }

            value = default;
            return false;
        }

        public bool TryPopLast(out T value)
        {
            if (_count > 0)
            {
                int next = _tail - 1 < 0 ? _capacity : _tail - 1; 
                value = _buffer[next];
                _tail = next;
                _count--;
                return true;
            }

            value = default;
            return false;
        }

        public bool TryPeekFirst(out T value)
        {
            if (_count > 0)
            {
                int next = (_head + 1) % _capacity;
                value = _buffer[next];
                return true;
            }

            value = default;
            return false;
        }

        public bool TryPeekLast(out T value)
        {
            if (_count > 0)
            {
                int next = _tail - 1 < 0 ? _capacity : _tail - 1;
                value = _buffer[next];
                return true;
            }

            value = default;
            return false;
        }

        public void Reverse()
        {
            var temp = _head;
            _head = _tail;
            _tail = temp;
        }

        public void TrimExcess()
        {
            TrimExcess(_count);
        }

        public void TrimExcess(int capacity)
        {
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

        public void EnsureCapcity(int capacity)
        {
            if(capacity <= 0)
            {
                throw new ArgumentException($"capacity should be greater than 0: {capacity}", nameof(capacity));
            }

            if(capacity > _capacity)
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
            if (_count == 0)
                return default;

            NativeArray<T> array = new NativeArray<T>(_count);
            void* src = _buffer;
            void* dst = array._buffer;
            Unsafe.CopyBlockUnaligned(dst, src, (uint)(sizeof(T) * _count));
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

        private void Resize()
        {
            Resize(_capacity * 2);
        }

        private void Resize(int capacity)
        {
            if (_buffer == null)
                return;

            T* newBuffer = GetAllocator()!.Allocate<T>(capacity);

            int next = _head;
            int i = 0;
            do
            {
                newBuffer[i] = _buffer[next];
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
            _capacity = capacity;
            _head = _count;
            _tail = 0;
        }

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
        /// Gets a string representation of the elements of this deque.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this deque elements.
        /// </returns>
        public override string ToString()
        {
            if (_buffer == null)
            {
                return "[Invalid]";
            }

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
        /// Gets an enumerator over the elements of the deque.
        /// </summary>
        /// <returns>An enumerator over this deque elements.</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(ref this);
        }

        public Span<T> Span => new Span<T>(_buffer, _capacity);

        /// <summary>
        /// An enumerator over the elements of a <see cref="NativeDeque{T}"/>.
        /// </summary>
        public ref struct Enumerator
        {
            private readonly void* _data;
            private readonly int _length;
            private readonly int _head;
            private readonly int _tail;
            private int _index;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(ref NativeDeque<T> queue)
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
