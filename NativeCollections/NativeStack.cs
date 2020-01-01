using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NativeCollections.Allocators;
using NativeCollections.Utility;

namespace NativeCollections
{
    unsafe public struct NativeStack<T> : IDisposable where T : unmanaged
    {
        internal T* _buffer;
        private int _capacity;
        private int _count;
        private int _allocatorID;

        public NativeStack(int initialCapacity) : this(initialCapacity, Allocator.Default) { }

        public NativeStack(int initialCapacity, Allocator allocator)
        {
            if (initialCapacity <= 0)
                throw new ArgumentException($"capacity must be greater than 0: {initialCapacity}");

            _buffer = allocator.Allocate<T>(initialCapacity);
            _capacity = initialCapacity;
            _count = 0;
            _allocatorID = allocator.ID;
        }

        public NativeStack(Span<int> elements) : this(elements, Allocator.Default) { }

        public NativeStack(Span<int> elements, Allocator allocator)
        {
            if (elements.IsEmpty)
            {
                this = default;
            }
            else
            {
                _buffer = allocator.Allocate<T>(elements.Length);
                _capacity = elements.Length;
                _count = _capacity;
                _allocatorID = allocator.ID;

                void* source = Unsafe.AsPointer(ref MemoryMarshal.GetReference(elements));
                Unsafe.CopyBlock(_buffer, source, (uint)(sizeof(T) * _capacity));
            }
        }

        public int Length => _count;

        public int Capacity => _capacity;

        public bool IsValid => _buffer != null;

        public bool IsEmpty => _count == 0;

        public Allocator? GetAllocator()
        {
            return Allocator.GetAllocatorByID(_allocatorID);
        }

        public void Push(T value)
        {
            if (_count == _capacity)
            {
                EnsureCapacity(_count + 1);
            }

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            Unsafe.Add(ref startAddress, _count++) = value;
        }

        public T Pop()
        {
            if (_count == 0)
                throw new InvalidOperationException("Stack is empty");

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            ref T value = ref Unsafe.Add(ref startAddress, _count);
            Unsafe.Add(ref startAddress, _count--) = default;
            return value;
        }

        public T Peek()
        {
            if (_count == 0)
                throw new InvalidOperationException("Stack is empty");

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            ref T value = ref Unsafe.Add(ref startAddress, _count);
            return value;
        }

        public bool TryPop(out T value)
        {
            if (_count == 0)
            {
                value = default;
                return false;
            }
            else
            {
                ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
                value = Unsafe.Add(ref startAddress, _count);
                Unsafe.Add(ref startAddress, _count--) = default;
                return true;
            }
        }

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
                value = Unsafe.Add(ref startAddress, _count);
                return true;
            }
        }

        public void Clear()
        {
            if (_count == 0)
                return;

            ref byte pointer = ref Unsafe.As<T, byte>(ref Unsafe.AsRef<T>(_buffer));
            uint length = (uint)(sizeof(T) * _count);
            Unsafe.InitBlockUnaligned(ref pointer, 0, length);
            _count = 0;
        }

        public bool Contains(T value)
        {
            return UnsafeUtilities.IndexOf(_buffer, 0, _count, value) >= 0;
        }

        public void TrimExcess()
        {
            if (_count != _capacity && _capacity > 4)
            {
                SetCapacity(_count);
            }
        }

        private void EnsureCapacity(int min)
        {
            if (min > _capacity)
            {
                if (min < _capacity * 2)
                {
                    SetCapacity(_capacity * 2);
                }
                else
                {
                    SetCapacity(min + 1);
                }
            }
        }

        private void SetCapacity(int newCapacity)
        {
            if (_buffer == null)
                return;

            newCapacity = newCapacity < 4 ? 4 : newCapacity;
            _buffer = GetAllocator()!.Reallocate(_buffer, newCapacity);
            _capacity = newCapacity;
        }

        public void Dispose()
        {
            if (_buffer == null)
                return;

            if (Allocator.IsCached(_allocatorID))
            {
                GetAllocator()!.Free(_buffer);
                _buffer = null;
                _capacity = 0;
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(ref this);
        }

        /// <summary>
        /// Exposes methods for iterate over the contents of a <see cref="NativeStack{T}"/>.
        /// </summary>
        /// <seealso cref="NativeCollections.INativeContainer{T}" />
        /// <seealso cref="System.IDisposable" />
        public ref struct Enumerator
        {
            private void* _pointer;
            private int _length;
            private int _index;

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> struct.
            /// </summary>
            /// <param name="list">The stack.</param>
            public Enumerator(ref NativeStack<T> stack)
            {
                _pointer = stack._buffer;
                _length = stack._count;
                _index = -1;
            }

            /// <summary>
            /// Gets a reference to the current value.
            /// </summary>
            /// <value>
            /// The current value.
            /// </value>
            /// <exception cref="ArgumentOutOfRangeException"></exception>
            public ref T Current
            {
                get
                {
                    if (_index < 0 || _index > _length)
                        throw new ArgumentOutOfRangeException("index", _index.ToString());

                    ref T pointer = ref Unsafe.AsRef<T>(_pointer);
                    return ref Unsafe.Add(ref pointer, _index);
                }
            }

            /// <summary>
            /// Disposes this enumerator.
            /// </summary>
            public void Dispose()
            {
                if (_pointer == null)
                    return;

                _pointer = null;
                _length = 0;
                _index = 0;
            }

            /// <summary>
            /// Moves to the next value.
            /// </summary>
            /// <returns><c>true</c> if has a next value, otherwise <c>false</c></returns>
            public bool MoveNext()
            {
                if (_pointer == null)
                    return false;

                int i = _index + 1;
                if (i < _length)
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
