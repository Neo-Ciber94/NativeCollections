using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NativeCollections.Allocators;
using NativeCollections.Utility;

namespace NativeCollections
{
    /// <summary>
    /// Represents a collection FILO (first-in last-out) where the last element added to the collection its the first to be removed.
    /// </summary>
    /// <typeparam name="T">Type of the elements.</typeparam>
    /// <seealso cref="NativeCollections.INativeContainer{T}" />
    /// <seealso cref="System.IDisposable" />
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeStackDebugView<>))]
    unsafe public struct NativeStack<T> : INativeContainer<T>, IDisposable where T : unmanaged
    {
        internal T* _buffer;
        private int _capacity;
        private int _count;

        private readonly int _allocatorID;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeStack{T}"/> struct.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        /// <exception cref="ArgumentException">If the capacity is negative or 0.</exception>
        public NativeStack(int initialCapacity) : this(initialCapacity, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeStack{T}"/> struct.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        /// <param name="allocator">The allocator.</param>
        /// <exception cref="ArgumentException">If the capacity is negative or 0.</exception>
        public NativeStack(int initialCapacity, Allocator allocator)
        {
            if (initialCapacity <= 0)
            {
                throw new ArgumentException($"capacity must be greater than 0: {initialCapacity}");
            }

            if (Allocator.IsCached(allocator) is false)
            {
                throw new ArgumentException("Allocator is not in cache.", nameof(allocator));
            }

            _buffer = allocator.Allocate<T>(initialCapacity);
            _capacity = initialCapacity;
            _count = 0;
            _allocatorID = allocator.ID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeStack{T}"/> struct.
        /// </summary>
        /// <param name="elements">The initial elements.</param>
        public NativeStack(Span<int> elements) : this(elements, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeStack{T}"/> struct.
        /// </summary>
        /// <param name="elements">The initial elements.</param>
        /// <param name="allocator">The allocator.</param>
        public NativeStack(Span<int> elements, Allocator allocator)
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
                _buffer = allocator.Allocate<T>(elements.Length);
                _capacity = elements.Length;
                _count = _capacity;
                _allocatorID = allocator.ID;

                void* source = Unsafe.AsPointer(ref MemoryMarshal.GetReference(elements));
                Unsafe.CopyBlock(_buffer, source, (uint)(sizeof(T) * _capacity));
            }
        }

        internal NativeStack(void* pointer, int length, Allocator allocator)
        {
            Debug.Assert(pointer != null);
            Debug.Assert(length > 0);
            Debug.Assert(Allocator.IsCached(allocator));

            _buffer = (T*)pointer;
            _capacity = length;
            _count = length;
            _allocatorID = allocator.ID;
        }

        private NativeStack(ref NativeStack<T> stack)
        {
            Debug.Assert(stack.IsValid);

            Allocator allocator = stack.GetAllocator()!;
            T* buffer = allocator.Allocate<T>(stack._capacity);
            Unsafe.CopyBlockUnaligned(buffer, stack._buffer, (uint)(sizeof(T) * stack._capacity));

            _buffer = buffer;
            _capacity = stack._capacity;
            _count = stack._count;
            _allocatorID = stack._allocatorID;
        }

        /// <summary>
        /// Gets the number of elements in the stack.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length => _count;

        /// <summary>
        /// Gets the number of elements this stack can hold before resize.
        /// </summary>
        /// <value>
        /// The capacity.
        /// </value>
        public int Capacity => _capacity;

        /// <summary>
        /// Checks if this stack is allocated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid => _buffer != null;

        /// <summary>
        /// Gets a value indicating whether this stack have elements.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => _count == 0;

        /// <summary>
        /// Gets the allocator used for this stack.
        /// </summary>
        /// <returns></returns>
        public Allocator? GetAllocator()
        {
            return Allocator.GetAllocatorByID(_allocatorID);
        }

        /// <summary>
        /// Adds a value at the end of the stack.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Push(T value)
        {
            if(_buffer == null)
            {
                throw new InvalidOperationException("NativeStack is invalid");
            }

            if (_count == _capacity)
            {
                RequireCapacity(_count + 1);
            }

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            Unsafe.Add(ref startAddress, _count++) = value;
        }

        /// <summary>
        /// Gets and removes the value at the top of the stack.
        /// </summary>
        /// <returns>The value at the top of the stack.</returns>
        /// <exception cref="InvalidOperationException">if the stack is empty</exception>
        public T Pop()
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeStack is invalid");
            }

            if (!TryPop(out T value))
            {
                throw new InvalidOperationException("Stack is empty");
            }

            return value;
        }

        /// <summary>
        /// Gets the value at the top of the stack.
        /// </summary>
        /// <returns>The value at the top of the stack.</returns>
        /// <exception cref="InvalidOperationException">if the stack is empty</exception>
        public T Peek()
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeStack is invalid");
            }

            if (!TryPeek(out T value))
            {
                throw new InvalidOperationException("Stack is empty");
            }

            return value;
        }

        /// <summary>
        /// Attemps to remove and get the value at the top of the stack.
        /// </summary>
        /// <param name="value">The value at the top of the stack.</param>
        /// <returns><c>true</c> if the value was removed, otherwise <c>false</c>.</returns>
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
                value = Unsafe.Add(ref startAddress, _count - 1);
                Unsafe.Add(ref startAddress, _count--) = default;
                return true;
            }
        }

        /// <summary>
        /// Attemps to get the value at the top of the stack.
        /// </summary>
        /// <param name="value">The value at the top of the stack.</param>
        /// <returns><c>true</c> if the value exists, otherwise <c>false</c>.</returns
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
                value = Unsafe.Add(ref startAddress, _count - 1);
                return true;
            }
        }

        /// <summary>
        /// Gets a reference to the value at the top of the stack.
        /// </summary>
        /// <returns>A reference to the value at the top of the stack.</returns>
        /// <exception cref="InvalidOperationException">if the stack is empty</exception>
        public ref T PeekReference()
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeStack is invalid");
            }

            if (_count == 0)
            {
                throw new InvalidOperationException("Stack is empty");
            }

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            return ref Unsafe.Add(ref startAddress, _count - 1);
        }

        /// <summary>
        /// Attemps to get a reference to the value at the top of the stack.
        /// </summary>
        /// <param name="value">A reference to the value at the top of the stack.</param>
        /// <returns><c>true</c> if the value exists, otherwise <c>false</c>.</returns
        public bool TryPeekReference(out ByReference<T> value)
        {
            if (_count == 0)
            {
                value = default;
                return false;
            }
            else
            {
                ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
                value = new ByReference<T>(ref Unsafe.Add(ref startAddress, _count - 1));
                return true;
            }
        }

        /// <summary>
        /// Removes the contents of the stack.
        /// </summary>
        public void Clear()
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeStack is invalid");
            }

            if (_count == 0)
            {
                return;
            }

            _count = 0;
        }

        /// <summary>
        /// Determines whether the stack contains the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the stack contains the specified value; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T value)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeStack is invalid");
            }

            return _count == 0 ? false : UnsafeUtilities.IndexOf(_buffer, 0, _count, value) >= 0;
        }

        /// <summary>
        /// Removes the excess spaces in the stack.
        /// </summary>
        public void TrimExcess()
        {
            TrimExcess(_count);
        }

        /// <summary>
        /// Removes the excess spaces in the stack.
        /// </summary>
        /// <param name="capacity">The min capacity.</param>
        public void TrimExcess(int capacity)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeStack is invalid");
            }

            if (capacity >= _count)
            {
                Resize(capacity);
            }
        }

        /// <summary>
        /// Ensures the stack can hold the specified number of elements without resize.
        /// </summary>
        /// <param name="capacity">The min capacity.</param>
        public void EnsureCapacity(int capacity)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeStack is invalid");
            }

            if (capacity > _capacity)
            {
                Resize(capacity);
            }
        }

        /// <summary>
        /// Copies the content of this stack to a <see cref="Span{T}" />.
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
                throw new InvalidOperationException("NativeStack is invalid");

            if (destinationIndex < 0 || destinationIndex > span.Length)
                throw new ArgumentOutOfRangeException(nameof(destinationIndex), destinationIndex.ToString());

            if (count < 0 || count > _count || count > (span.Length - destinationIndex))
                throw new ArgumentOutOfRangeException(nameof(count), count.ToString());

            void* dst = (T*)Unsafe.AsPointer(ref span.GetPinnableReference()) + destinationIndex;
            void* src = _buffer;
            Unsafe.CopyBlock(dst, src, (uint)(sizeof(T) * count));
        }

        /// <summary>
        /// Releases the resources used for this stack.
        /// </summary>
        public void Dispose()
        {
            if (_buffer == null)
            {
                return;
            }

            GetAllocator()!.Free(_buffer);
            this = default;
        }

        /// <summary>
        /// Gets the pointer to this stack elements.
        /// </summary>
        /// <returns>A pointer to this buffer elements.</returns>
        public T* GetUnsafePointer()
        {
            return _buffer;
        }

        /// <summary>
        /// Allocates an array with the elements of this stack.
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
        /// Creates a new <see cref="NativeArray{T}"/> with the elements of this stack.
        /// </summary>
        /// <returns>A new array with the elements of this instance.</returns>
        public NativeArray<T> ToNativeArray()
        {
            if (_count == 0)
            {
                return default;
            }

            NativeArray<T> array = new NativeArray<T>(_count, GetAllocator()!);
            void* src = _buffer;
            void* dst = array._buffer;
            Unsafe.CopyBlockUnaligned(dst, src, (uint)(sizeof(T) * _count));
            return array;
        }

        /// <summary>
        /// Creates a new <see cref="NativeArray{T}"/> with the elements of this stack and dispose this stack.
        /// </summary>
        /// <param name="createNewArrayIfNeeded">If <c>true</c> a new array will be created if the capacity of this
        /// stack is different than its length; otherwise is guaranteed the new array will use this stack memory.</param>
        /// <returns>A newly created array with this list elements.</returns>
        public NativeArray<T> ToNativeArrayAndDispose(bool createNewArrayIfNeeded = true)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeStack is invalid");
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
        /// Gets a string representation of the elements of this stack.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this stack elements.
        /// </returns>
        public override string ToString()
        {
            if (_count == 0)
            {
                return "[]";
            }

            StringBuilder sb = StringBuilderCache.Acquire();
            sb.Append('[');

            for (int i = 0; i < _count; ++i)
            {
                sb.Append(_buffer[i].ToString());

                if (i < _count - 1)
                {
                    sb.Append(',');
                    sb.Append(' ');
                }
            }

            sb.Append(']');
            return StringBuilderCache.ToStringAndRelease(ref sb!);
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
            Debug.Assert(_buffer != null);
            newCapacity = newCapacity < 4 ? 4 : newCapacity;
            _buffer = GetAllocator()!.Reallocate(_buffer, newCapacity);
            _capacity = newCapacity;
        }

        /// <summary>
        /// Gets an enumerator over the elements of the stack.
        /// </summary>
        /// <returns>An enumerator over the elements of the stack.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RefEnumerator<T> GetEnumerator()
        {
            return _buffer == null? default : new RefEnumerator<T>(_buffer, _count);
        }
    }
}
