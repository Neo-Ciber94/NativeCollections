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
    /// Represents a collection of objects of the same type allocated with unmanaged memory that must be disponse after use,
    /// also offers operations for add, remove or query over the elements of the list.
    /// </summary>
    /// <typeparam name="T">Type of the objects</typeparam>
    /// <seealso cref="NativeCollections.INativeContainer{T}" />
    /// <seealso cref="System.IDisposable" />
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeListDebugView<>))]
    unsafe public struct NativeList<T> : INativeContainer<T>, IDisposable where T : unmanaged
    {
        internal T* _buffer;
        private int _capacity;
        private int _count;
        private int _allocatorID;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeList{T}"/> struct.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the list.</param>
        /// <exception cref="ArgumentException">If the capacity is negative or zero.</exception>
        public NativeList(int initialCapacity) : this(initialCapacity, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeList{T}"/> struct.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the list.</param>
        /// <param name="allocator">The allocator used for this list.</param>
        /// <exception cref="ArgumentException">If the capacity is negative or zero.</exception>
        public NativeList(int initialCapacity, Allocator allocator)
        {
            if (initialCapacity <= 0)
            {
                throw new ArgumentException($"initialCapacity should be greater than 0: {initialCapacity}");
            }

            if (allocator.ID <= 0)
            {
                throw new ArgumentException("Allocator is not in cache.", "allocator");
            }

            _buffer = allocator.Allocate<T>(initialCapacity);
            _capacity = initialCapacity;
            _count = 0;
            _allocatorID = allocator.ID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeList{T}"/> struct using the given elements.
        /// </summary>
        /// <param name="elements">The initial elements of the list.</param>
        public NativeList(Span<T> elements) : this(elements, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeList{T}"/> struct using the given elements.
        /// </summary>
        /// <param name="elements">The initial elements of the list.</param>
        /// <param name="allocator">The allocator used for this list.</param>
        public NativeList(Span<T> elements, Allocator allocator)
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
                _buffer = allocator.Allocate<T>(elements.Length);
                _capacity = elements.Length;
                _count = _capacity;
                _allocatorID = allocator.ID;

                void* source = Unsafe.AsPointer(ref MemoryMarshal.GetReference(elements));
                Unsafe.CopyBlock(_buffer, source, (uint)(Unsafe.SizeOf<T>() * _capacity));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeList{T}"/> struct using the specified pointer.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="length">The number of elements in the pointer.</param>
        /// <exception cref="ArgumentException">If the pointer is null or length is lower or equals to zero.
        /// </exception>
        public NativeList(void* pointer, int length)
        {
            if (pointer == null)
                throw new ArgumentException("Invalid pointer");

            if (length <= 0)
                throw new ArgumentException($"Invalid length: {length}", nameof(length));

            _buffer = (T*)pointer;
            _capacity = length;
            _count = length;
            _allocatorID = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeList{T}"/> struct using the specified pointer.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="length">The number of elements in the pointer.</param>
        /// <param name="allocator">The allocator used for this list.</param>
        /// <exception cref="ArgumentException">If the pointer is null or length is lower or equals to zero.
        /// </exception>
        public NativeList(void* pointer, int length, Allocator allocator)
        {
            if (allocator.ID <= 0)
            {
                throw new ArgumentException("Allocator is not in cache.", "allocator");
            }

            if (pointer == null)
                throw new ArgumentException("Invalid pointer");

            if (length <= 0)
                throw new ArgumentException($"Invalid length: {length}", nameof(length));

            _buffer = (T*)pointer;
            _capacity = length;
            _count = length;
            _allocatorID = allocator.ID;
        }

        /// <summary>
        /// Gets the number of elements in the list.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length => _count;

        /// <summary>
        /// Gets the number of elements this list can hold before resize.
        /// </summary>
        /// <value>
        /// The capacity.
        /// </value>
        public int Capacity
        {
            get => _capacity;
        }

        /// <summary>
        /// Checks if this list is allocated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is allocated; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid => _buffer != null;

        /// <summary>
        /// Gets a value indicating whether this instance have elements.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => _count == 0;

        /// <summary>
        /// Gets the allocator used for this list.
        /// </summary>
        /// <returns>
        /// The allocator.
        /// </returns>
        public Allocator? GetAllocator()
        {
            return Allocator.GetAllocatorByID(_allocatorID);
        }

        /// <summary>
        /// Gets a reference to the element at the specified index.
        /// </summary>
        /// <value>
        /// The type of the object.
        /// </value>
        /// <param name="index">The index of the element.</param>
        /// <returns>A reference to the value</returns>
        public ref T this[int index]
        {
            get
            {
                Debug.Assert(_buffer != null, "NativeList is invalid");

                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException("index", $"{index}");

                ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
                return ref Unsafe.Add(ref startAddress, index);
            }
        }

        /// <summary>
        /// Gets a reference to the element at the specified index.
        /// </summary>
        /// <value>
        /// The type of the object.
        /// </value>
        /// <param name="index">The index of the element.</param>
        /// <returns>A reference to the value</returns>
        public ref T this[Index index]
        {
            get
            {
                Debug.Assert(_buffer != null, "NativeList is invalid");

                int i = index.IsFromEnd ? _count - index.Value - 1 : index.Value;

                if (i < 0 || i >= _count)
                    throw new ArgumentOutOfRangeException("index", $"{index}");

                ref T pointer = ref Unsafe.AsRef<T>(_buffer);
                return ref Unsafe.Add(ref pointer, i);
            }
        }

        /// <summary>
        /// Adds the specified value at the end of the list.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Add(T value)
        {
            Debug.Assert(_buffer != null, "NativeList is invalid");

            if (_count == _capacity)
            {
                RequireCapacity(_count + 1);
            }

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            Unsafe.Add(ref startAddress, _count++) = value;
        }

        /// <summary>
        /// Adds all the elements in the span at the end of the list.
        /// </summary>
        /// <param name="elements">The span that holds the elements to add.</param>
        public void AddRange(Span<T> elements)
        {
            if (elements.IsEmpty)
                throw new ArgumentException("Empty span");

            void* startAddress = Unsafe.AsPointer(ref MemoryMarshal.GetReference(elements));
            AddRange(startAddress, elements.Length);
        }

        /// <summary>
        /// Adds all the elements in the array at the end of the list.
        /// </summary>
        /// <param name="elements">The array that holds the elements to add.</param>
        public void AddRange(NativeArray<T> elements)
        {
            if (elements.IsValid)
                throw new ArgumentException("Invalid array");

            AddRange(elements._buffer, elements.Length);
        }

        private void AddRange(void* source, int length)
        {
            Debug.Assert(_buffer != null, "NativeList is invalid");

            if (source == null)
                throw new ArgumentException("Invalid startAddress");

            if (length <= 0)
                throw new ArgumentException(nameof(length), length.ToString());

            RequireCapacity(_count + length);

            void* destination = ((byte*)_buffer) + Unsafe.SizeOf<T>() * _count;
            Unsafe.CopyBlock(destination, source, (uint)(length * Unsafe.SizeOf<T>()));
            _count += length;
        }

        /// <summary>
        /// Inserts the value at the specified index.
        /// </summary>
        /// <param name="index">The index where insert the value.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the index is lower than 0 or greater than Length.</exception>
        public void Insert(int index, T value)
        {
            InsertRange(index, Unsafe.AsPointer(ref value), 1);
        }

        /// <summary>
        /// Inserts all the elements in the span at the specified index.
        /// </summary>
        /// <param name="index">The index where insert the elements.</param>
        /// <param name="elements">The span that holds the elements.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the index is lower than 0 or greater than Length.</exception>
        public void InsertRange(int index, Span<T> elements)
        {
            if (_buffer == null)
                return;

            if (elements.IsEmpty)
                throw new ArgumentException("Empty span");

            void* pointer = Unsafe.AsPointer(ref MemoryMarshal.GetReference(elements));
            InsertRange(index, pointer, elements.Length);
        }

        /// <summary>
        /// Inserts all the elements in the array at the specified index.
        /// </summary>
        /// <param name="index">The index where insert the elements.</param>
        /// <param name="elements">The array that holds the elements.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the index is lower than 0 or greater than Length.</exception>
        public void InsertRange(int index, NativeArray<T> elements)
        {
            if (_buffer == null)
                return;

            if (!elements.IsValid)
                throw new ArgumentException("Invalid array");
           
            InsertRange(index, elements._buffer, elements.Length);
        }

        private void InsertRange(int index, void* source, int length)
        {
            Debug.Assert(_buffer != null, "NativeList is invalid");

            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException("index", index.ToString());

            if (source == null)
                throw new ArgumentException("Invalid pointer");

            if (length <= 0)
                throw new ArgumentException(nameof(length), length.ToString());

            RequireCapacity(_count + length);

            int size = _count - index;
            void* targetPtr = _buffer + index;
            void* offset = _buffer + (index + length);
            Unsafe.CopyBlock(offset, targetPtr, (uint)(sizeof(T) * size));
            Unsafe.CopyBlock(targetPtr, source, (uint)(sizeof(T) * length));
            _count += length;
        }

        /// <summary>
        /// Removes the specified value from the list.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        /// <returns><c>true</c> if the value is removed; otherwise <c>false</c>.</returns>
        public bool Remove(T value)
        {
            int index = IndexOf(value);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the elements at the specified range.
        /// </summary>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the range is invalid</exception>
        public void RemoveRange(int start, int end)
        {
            Debug.Assert(_buffer != null, "NativeList is invalid");

            if (start < 0 || start > end || start > _count)
                throw new ArgumentOutOfRangeException(nameof(start), $"start: {start}, end: {end}");

            if (end < 0 || end > _count)
                throw new ArgumentOutOfRangeException(nameof(end), $"start: {start}, end: {end}");

            if (start == 0 && end == _count)
            {
                Clear();
                return;
            }

            int length = _count - end;
            int removed = end - start;

            void* src = _buffer + end;
            void* dst = _buffer + start;
            Unsafe.CopyBlock(dst, src, (uint)(sizeof(T) * length));
            _count -= removed;
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The index of the element.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the index is negative or greater than Length.</exception>
        public void RemoveAt(int index)
        {
            Debug.Assert(_buffer != null, "NativeList is invalid");

            if (index < 0 || index > _count)
                throw new ArgumentOutOfRangeException("index", $"{index}");

            _count--;

            if(index < _count)
            {
                void* src = _buffer + index + 1;
                void* dst = _buffer + index;
                Unsafe.CopyBlock(dst, src, (uint)(sizeof(T) * (_count - index + 1)));
            }

            _buffer[_count] = default;
        }

        /// <summary>
        /// Removes all the elements that meet the specified condition.
        /// </summary>
        /// <param name="predicate">The condition.</param>
        /// <returns>The number of elements removed</returns>
        public int RemoveIf(Predicate<T> predicate)
        {
            Debug.Assert(_buffer != null, "NativeList is invalid");

            int removedCount = 0;
            for (int i = 0; i < _count; i++)
            {
                ref T value = ref ((T*)_buffer)[i];
                if (predicate(value))
                {
                    RemoveAt(i--);
                    removedCount++;
                }
            }

            return removedCount;
        }

        /// <summary>
        /// Replaces all the elements equals to <c>value</c> with the <c>newValue</c>.
        /// </summary>
        /// <param name="value">The value to replace.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>The number of elements replaced</returns>
        public int ReplaceAll(T value, T newValue)
        {
            if (_buffer == null)
                return 0;

            return UnsafeUtilities.ReplaceAll(_buffer, 0, _count, value, newValue);
        }

        /// <summary>
        /// Replaces all the elements that match the specified condition.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="predicate">The condition.</param>
        /// <returns>The number of elements replaced.</returns>
        public int ReplaceIf(T newValue, Predicate<T> predicate)
        {
            Debug.Assert(_buffer != null, "NativeList is invalid");

            int count = 0;
            for (int i = 0; i < _count; i++)
            {
                ref T value = ref _buffer[i];
                if (predicate(value))
                {
                    value = newValue;
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Clears the content of this list.
        /// </summary>
        public void Clear()
        {
            if (_count == 0)
                return;

            ref byte startAddress = ref Unsafe.As<T, byte>(ref Unsafe.AsRef<T>(_buffer));
            uint length = (uint)(Unsafe.SizeOf<T>() * _count);
            Unsafe.InitBlockUnaligned(ref startAddress, 0, length);
            _count = 0;
        }

        /// <summary>
        /// Reverses the order of the content of this list.
        /// </summary>
        public void Reverse()
        {
            Reverse(0, _count - 1);
        }

        /// <summary>
        /// Reverses the order of the content of this list.
        /// </summary>
        /// <param name="start">The start index.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the range is invalid.</exception>
        public void Reverse(int start)
        {
            Reverse(start, _count - 1);
        }

        /// <summary>
        /// Reverses the order of the content of this list.
        /// </summary>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the range is invalid.</exception>
        public void Reverse(int start, int end)
        {
            Debug.Assert(_buffer != null, "NativeList is invalid");

            if (start < 0 || start > end || start > _count)
                throw new ArgumentOutOfRangeException(nameof(start), $"start: {start}, end: {end}");

            if (end < 0 || end > _count)
                throw new ArgumentOutOfRangeException(nameof(end), $"start: {start}, end: {end}");

            UnsafeUtilities.Reverse<T>(_buffer, start, end);
        }

        /// <summary>
        /// Gets the index of the specified element.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <returns>The index of the first match value or -1 if not found.</returns>
        public int IndexOf(T value)
        {
            return IndexOf(value, 0, _count);
        }

        /// <summary>
        /// Gets the index of the specified element.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <param name="start">The start index.</param>
        /// <returns>The index of the first match value or -1 if not found.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the range is invalid.</exception>
        public int IndexOf(T value, int start)
        {
            return IndexOf(value, start, _count);
        }

        /// <summary>
        /// Gets the index of the specified element.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <returns>The index of the first match value or -1 if not found.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the range is invalid.</exception>
        public int IndexOf(T value, int start, int end)
        {
            Debug.Assert(_buffer != null, "NativeList is invalid");

            if (_count == 0)
                return -1;

            if (start < 0 || start > end || start > _count)
                throw new ArgumentOutOfRangeException(nameof(start), $"start: {start}, end: {end}");

            if (end < 0 || end > _count)
                throw new ArgumentOutOfRangeException(nameof(end), $"start: {start}, end: {end}");

            return UnsafeUtilities.IndexOf(_buffer, start, end, value);
        }
     
        /// <summary>
        /// Gets the index of the specified element.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <returns>The index of the last match value or -1 if not found.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the range is invalid.</exception>
        public int LastIndexOf(T value)
        {
            return LastIndexOf(value, 0, _count);
        }

        /// <summary>
        /// Gets the index of the specified element.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <param name="start">The start index.</param>
        /// <returns>The index of the last match value or -1 if not found.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the range is invalid.</exception>
        public int LastIndexOf(T value, int start)
        {
            return LastIndexOf(value, start, _count);
        }
        
        /// <summary>
        /// Gets the index of the specified element.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <returns>The index of the last match value or -1 if not found.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the range is invalid.</exception>
        public int LastIndexOf(T value, int start, int end)
        {
            Debug.Assert(_buffer != null, "NativeList is invalid");

            if (_count == 0)
                return -1;

            if (start < 0 || start > end || start > _count)
                throw new ArgumentOutOfRangeException(nameof(start), $"start: {start}, end: {end}");

            if (end < 0 || end > _count)
                throw new ArgumentOutOfRangeException(nameof(end), $"start: {start}, end: {end}");

            return UnsafeUtilities.LastIndexOf(_buffer, start, end, value);
        }

        /// <summary>
        /// Determines whether this list contains the specified value.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <returns>
        ///   <c>true</c> if contains the value; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T value)
        {
            return IndexOf(value) >= 0;
        }

        /// <summary>
        /// Determines whether this list contains all specified values.
        /// </summary>
        /// <param name="elements">The elements to locate.</param>
        /// <returns>
        ///   <c>true</c> if contains all the elements; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsAll(Span<T> elements)
        {
            if (elements.Length == 0)
                return false;

            int count = 0;
            foreach(var e in elements)
            {
                if (Contains(e))
                {
                    count++;
                }
            }

            return count == elements.Length;
        }

        /// <summary>
        /// Copies the content of this list to a <see cref="System.Span{T}" />.</summary>
        /// <param name="span">The destination span to copy the data.</param>
        public readonly void CopyTo(in Span<T> span)
        {
            CopyTo(span, 0, _count);
        }

        /// <summary>
        /// Copies the content of this list to a <see cref="System.Span{T}" />.</summary>
        /// <param name="span">The destination span to copy the data.</param>
        /// <param name="count">The number of elements to copy.</param>
        public readonly void CopyTo(in Span<T> span, int count)
        {
            CopyTo(span, 0, count);
        }

        /// <summary>
        /// Copies the content of this list to a <see cref="System.Span{T}" />.</summary>
        /// <param name="span">The destination span to copy the data.</param>
        /// <param name="destinationIndex">Start index of the destination where start to copy.</param>
        /// <param name="count">The number of elements to copy.</param>
        public readonly void CopyTo(in Span<T> span, int destinationIndex, int count)
        {
            if (span.IsEmpty)
                throw new ArgumentException("Span is empty");

            if (_buffer == null)
                throw new InvalidOperationException("NativeList is invalid");

            if (destinationIndex < 0 || destinationIndex > span.Length)
                throw new ArgumentOutOfRangeException(nameof(destinationIndex), destinationIndex.ToString());

            if (count < 0 || count > _count || count > (span.Length - destinationIndex))
                throw new ArgumentOutOfRangeException(nameof(count), count.ToString());

            void* dst = (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span)) + destinationIndex;
            void* src = _buffer;
            Unsafe.CopyBlock(dst, src, (uint)(sizeof(T) * count));
        }

        /// <summary>
        /// Allocates a new array with the elements of this list.
        /// </summary>
        /// <returns>An array with the elements of this list.</returns>
        public T[] ToArray()
        {
            if (_buffer == null)
            {
                return Array.Empty<T>();
            }

            T[] array = new T[_count];
            CopyTo(array);
            return array;
        }

        /// <summary>
        /// Allocates a new <see cref="NativeArray"/> with the elements of this list.
        /// </summary>
        /// <returns>A newly create array with this list elements.</returns>
        public NativeArray<T> ToNativeArray()
        {
            if (_buffer == null)
            {
                return default;
            }

            NativeArray<T> array = new NativeArray<T>(_count, GetAllocator()!);
            Unsafe.CopyBlock(array._buffer, _buffer, (uint)(sizeof(T) * _count));
            return array;
        }

        /// <summary>
        /// Creates a new <see cref="NativeArray{T}"/> with the elements of this list and dispose this list.
        /// </summary>
        /// <param name="createNewArrayIfNeeded">If <c>true</c> a new array will be created if the capacity of this
        /// list is different than its length; otherwise is guaranteed the new array will use this list memory.</param>
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
        /// Gets a pointer to the elements of this list.
        /// </summary>
        /// <returns>A pointer to this list elements.</returns>
        public T* GetUnsafePointer()
        {
            return (T*)_buffer;
        }

        /// <summary>
        /// Release the allocated resources of this list.
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
        /// Removes the excess capacity of this list so it keep only enough space for hold its elements.
        /// </summary>
        public void TrimExcess()
        {
            TrimExcess(_count);
        }

        /// <summary>
        /// Removes the excess capacity of this list so it keep only enough space for hold its elements.
        /// </summary>
        /// <param name="capacity">The expected capacity.</param>
        public void TrimExcess(int capacity)
        {
            Debug.Assert(_buffer != null, "NativeList is invalid");

            if (capacity < _count)
            {
                if (capacity <= 0)
                {
                    throw new ArgumentException("capacity must be greater than 0.");
                }

                return;
            }

            Resize(capacity);
        }

        /// <summary>
        /// Ensures this list have enough capacity for hold elements without resize.
        /// </summary>
        /// <param name="capacity">The expected min capacity.</param>
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

            // newCapacity = newCapacity < 4 ? 4 : newCapacity;
            _buffer = GetAllocator()!.Reallocate(_buffer, newCapacity);
            _capacity = newCapacity;
        }

        /// <summary>
        /// Gets an string representation of the elements of this list.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents the elements of this instance.
        /// </returns>
        public override string ToString()
        {
            if(_buffer == null)
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
                    ref T value = ref enumerator.Current;
                    sb.Append(value.ToString());

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
        /// Gets an enumerator for iterate over the elements of this list.
        /// </summary>
        /// <returns>An enumerator over this list elements.</returns>
        public Enumerator GetEnumerator()
        {
            Debug.Assert(_buffer != null, "NativeList is invalid");

            return new Enumerator(ref this);
        }

        /// <summary>
        /// Exposes methods for iterate over the contents of a <see cref="NativeList{T}"/>.
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
            /// <param name="list">The list.</param>
            public Enumerator(ref NativeList<T> list)
            {
                _pointer = list._buffer;
                _length = list._count;
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
