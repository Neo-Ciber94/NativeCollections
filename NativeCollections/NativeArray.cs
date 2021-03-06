﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NativeCollections.Allocators;
using NativeCollections.Utility;

namespace NativeCollections
{
    /// <summary>
    /// Represents a collection of objects of the same type allocated with unmanaged memory that must be disponse after use.
    /// </summary>
    /// <typeparam name="T">Type of the objects</typeparam>
    /// <seealso cref="NativeCollections.INativeContainer{T}" />
    /// <seealso cref="System.IDisposable" />
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeArrayDebugView<>))]
    unsafe public struct NativeArray<T> : INativeContainer<T>, IDisposable where T : unmanaged
    {
        internal T* _buffer;
        private int _capacity;
        private readonly int _allocatorID;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeArray{T}" /> struct.
        /// </summary>
        /// <param name="capacity">The capacity of the array.</param>
        public NativeArray(int capacity) : this(capacity, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeArray{T}" /> struct.
        /// </summary>
        /// <param name="capacity">The capacity of the array.</param>
        /// <param name="allocator">The allocator used for this array.</param> 
        public NativeArray(int capacity, Allocator allocator)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException($"capacity must be greater than 0: {capacity}");
            }

            if(allocator.ID <= 0)
            {
                throw new ArgumentException("Allocator is not in cache.", "allocator");
            }

            _buffer = allocator.Allocate<T>(capacity);
            _capacity = capacity;
            _allocatorID = allocator.ID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeArray{T}" /> struct by copying the specify elements.
        /// </summary>
        /// <param name="elements">The elements to initializate the array.</param>
        public NativeArray(in Span<T> elements) : this(elements, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeArray{T}" /> struct by copying the specify elements.
        /// </summary>
        /// <param name="elements">The elements to initializate the array.</param>
        /// <param name="allocator">The allocator used for this array.</param> 
        public NativeArray(in Span<T> elements, Allocator allocator)
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
                _allocatorID = allocator.ID;

                void* source = Unsafe.AsPointer(ref MemoryMarshal.GetReference(elements));
                Unsafe.CopyBlock(_buffer, source, (uint)(sizeof(T) * _capacity));
            }
        }

        internal NativeArray(void* pointer, int length, Allocator allocator)
        {
            if (pointer == null)
            {
                throw new ArgumentException("Invalid pointer");
            }

            if (length <= 0)
            {
                throw new ArgumentException($"Invalid length: {length}", nameof(length));
            }

            if (allocator.ID <= 0)
            {
                throw new ArgumentException("Allocator is not in cache.", "allocator");
            }

            _buffer = (T*)pointer;
            _capacity = length;
            _allocatorID = allocator.ID;
        }

        /// <summary>
        /// Gets the number of elements in this array.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length => _capacity;

        /// <summary>
        /// Checks if this array is allocated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is allocated; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid => _buffer != null;

        /// <summary>
        /// Gets a value indicating whether this array has no elements.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this array is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => _capacity == 0;

        /// <summary>
        /// Gets the allocator used for this array.
        /// </summary>
        /// <returns>
        /// The allocator.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                if (_buffer == null)
                {
                    throw new InvalidOperationException("NativeArray is invalid");
                }

                if (index < 0 || index >= _capacity)
                    throw new ArgumentOutOfRangeException("index", $"{index}");

                return ref _buffer[index];
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
                int i = index.IsFromEnd ? _capacity - index.Value - 1: index.Value;
                return ref this[i];
            }
        }

        /// <summary>
        /// Gets a slice from the specified range.
        /// </summary>
        /// <value>
        /// A <see cref="NativeSlice{T}" /> from this array.
        /// </value>
        /// <param name="range">The range.</param>
        /// <returns>A slice from this array within the given range.</returns>
        public NativeSlice<T> this[Range range]
        {
            get
            {
                if (_buffer == null)
                {
                    throw new InvalidOperationException("NativeArray is invalid");
                }

                var (index, length) = range.GetOffsetAndLength(_capacity);

                if (index < 0 || index >= _capacity)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index.ToString());
                }

                if (length < 0 || length > (_capacity - index))
                {
                    throw new ArgumentOutOfRangeException(nameof(length), length.ToString());
                }

                byte* buffer = (byte*)_buffer + (sizeof(T) * index);
                return new NativeSlice<T>(buffer, length);
            }
        }

        /// <summary>
        /// Fills the content of this array with the given value.
        /// </summary>
        /// <param name="value">The value to use.</param>
        public void Fill(T value)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeArray is invalid");
            }

            if (sizeof(T) == 1)
            {
                ref byte value2 = ref Unsafe.As<T, byte>(ref value);
                Unsafe.InitBlock(_buffer, value2, (uint)_capacity);
            }
            else
            {
                T* pointer = _buffer;
                int count = _capacity;
                int pos = 0;

                while (count >= 8)
                {
                    pointer[pos] = value;
                    pointer[pos + 1] = value;
                    pointer[pos + 2] = value;
                    pointer[pos + 3] = value;
                    pointer[pos + 4] = value;
                    pointer[pos + 5] = value;
                    pointer[pos + 6] = value;
                    pointer[pos + 7] = value;
                    count -= 8;
                    pos += 8;
                }

                while (count >= 4)
                {
                    pointer[pos] = value;
                    pointer[pos + 1] = value;
                    pointer[pos + 2] = value;
                    pointer[pos + 3] = value;
                    count -= 4;
                    pos += 4;
                }

                for (; count > 0; --count)
                {
                    pointer[pos++] = value;
                }
            }
        }

        /// <summary>
        /// Resizes this array.
        /// </summary>
        /// <param name="newCapacity">The new capacity.</param>
        /// <exception cref="ArgumentException">If newCapacity is negative or zero.</exception>
        public void Resize(int newCapacity)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeArray is invalid");
            }

            if (newCapacity <= 0)
            {
                throw new ArgumentException("newCapacity cannot be negative or zero", nameof(newCapacity));
            }

            if (newCapacity == _capacity)
            {
                return;
            }

            _buffer = GetAllocator()!.Reallocate<T>(_buffer, newCapacity);
            _capacity = newCapacity;
        }

        /// <summary>
        /// Reverses the order of the content of this array.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reverse()
        {
            Reverse(0, _capacity - 1);
        }

        /// <summary>
        /// Reverses the order of the content of this array.
        /// </summary>
        /// <param name="start">The start index.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reverse(int start)
        {
            Reverse(start, _capacity - 1);
        }

        /// <summary>
        /// Reverses the order of the content of this array.
        /// </summary>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        public void Reverse(int start, int end)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeArray is invalid");
            }

            if (start < 0 || end < 0 || start > end || start > _capacity || end > _capacity)
            {
                throw new ArgumentOutOfRangeException($"Invalid range; start: {start}, end: {end}");
            }

            UnsafeUtilities.Reverse<T>(_buffer, start, end);
        }

        /// <summary>
        /// Determines whether this array contains the specified value.
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <returns>
        ///   <c>true</c> if the array contains the value; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T value)
        {
            return IndexOf(value) >= 0;
        }

        /// <summary>
        /// Get the index of the specified value.
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <returns>The index of the element or -1 if not found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T value)
        {
            return IndexOf(value, 0, _capacity);
        }

        /// <summary>
        /// Get the index of the specified value.
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <param name="start">The start index of the search.</param>
        /// <returns>The index of the element or -1 if not found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T value, int start)
        {
            return IndexOf(value, start, _capacity);
        }

        /// <summary>
        /// Get the index of the specified value.
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <param name="start">The start index of the search.</param>
        /// <param name="end">The end index of the search.</param>
        /// <returns>The index of the element or -1 if not found.</returns>
        public int IndexOf(T value, int start, int end)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeArray is invalid");
            }

            if (start < 0 || end < 0 || start > end || start > _capacity || end > _capacity)
            {
                throw new ArgumentOutOfRangeException($"Invalid range; start: {start}, end: {end}");
            }

            return UnsafeUtilities.IndexOf(_buffer, start, end, value);
        }

        /// <summary>
        /// Get the index of the last specified value.
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <returns>The index of the element or -1 if not found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(T value)
        {
            return LastIndexOf(value, 0, _capacity);
        }

        /// <summary>
        /// Get the index of the last specified value.
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <param name="start">The start index of the search.</param>
        /// <returns>The index of the element or -1 if not found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(T value, int start)
        {
            return LastIndexOf(value, start, _capacity);
        }

        /// <summary>
        /// Get the index of the last specified value.
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <param name="start">The start index of the search.</param>
        /// <param name="end">The end index of the search.</param>
        /// <returns>The index of the element or -1 if not found.</returns>
        public int LastIndexOf(T value, int start, int end)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeArray is invalid");
            }

            if (start < 0 || end < 0 || start > end || start > _capacity || end > _capacity)
            {
                throw new ArgumentOutOfRangeException($"Invalid range; start: {start}, end: {end}");
            }

            return UnsafeUtilities.LastIndexOf(_buffer, start, end, value);
        }

        /// <summary>
        /// Copies the content of this array into the specified array.
        /// </summary>
        /// <param name="span">The span to copy the elements.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void CopyTo(in Span<T> span)
        {
            CopyTo(span, 0, 0, _capacity);
        }

        /// <summary>
        /// Copies the content of this array into the specified array.
        /// </summary>
        /// <param name="span">The span to copy the elements.</param>
        /// <param name="count">The number of elements to copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void CopyTo(in Span<T> span, int count)
        {
            CopyTo(span, 0, 0, count);
        }

        /// <summary>
        /// Copies the content of this array into the specified array.
        /// </summary>
        /// <param name="span">The span to copy the elements.</param>
        /// <param name="destinationIndex">Index of where copy the elements in the span.</param>
        /// <param name="count">The number of elements to copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void CopyTo(in Span<T> span, int destinationIndex, int count)
        {
            CopyTo(span, 0, destinationIndex, count);
        }

        /// <summary>
        /// Copies the content of this array into the specified array.
        /// </summary>
        /// <param name="span">The span to copy the elements.</param>
        /// <param name="sourceIndex">Index where start the copy in the array.</param>
        /// <param name="destinationIndex">Index of where copy the elements in the span.</param>
        /// <param name="count">The number of elements to copy.</param>
        public readonly void CopyTo(in Span<T> span, int sourceIndex, int destinationIndex, int count)
        {
            if (span.IsEmpty)
                throw new ArgumentException("Span is empty", nameof(span));

            if (_buffer == null)
                throw new InvalidOperationException("NativeArray is invalid");

            if (sourceIndex < 0 || sourceIndex > _capacity)
                throw new ArgumentOutOfRangeException(nameof(sourceIndex), sourceIndex.ToString());

            if (destinationIndex < 0 || destinationIndex > span.Length)
                throw new ArgumentOutOfRangeException(nameof(sourceIndex), sourceIndex.ToString());

            if (count < 0 || count > (_capacity - sourceIndex) || count > (span.Length - destinationIndex))
                throw new ArgumentException(count.ToString(), nameof(count));

            T* destination = (T*)Unsafe.AsPointer(ref span.GetPinnableReference()) + destinationIndex;
            T* source = (T*)_buffer + sourceIndex;
            Unsafe.CopyBlock(destination, source, (uint)(sizeof(T) * count));
        }

        /// <summary>
        /// Allocates a new array with the content of this array.
        /// </summary>
        /// <returns>An array with the elements of this</returns>
        public T[] ToArray()
        {
            if (_capacity == 0)
            {
                return Array.Empty<T>();
            }

            T[] array = new T[_capacity];
            CopyTo(array);
            return array;
        }

        /// <summary>
        /// Releases the allocated memory of this array.
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
        /// Creates a new <see cref="NativeList{T}"/> that will owns this array memory, and then invalidates this array.
        /// </summary>
        /// <returns>A newly created NativeList that owns this array memory.</returns>
        public NativeList<T> ToNativeListAndDispose()
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeArray is invalid");
            }

            // The new NativeList will owns this NativeArray memory
            NativeList<T> list = new NativeList<T>(_buffer, _capacity, GetAllocator()!);

            // Invalidate this NativeArray, not actual dispose
            this = default;

            return list;
        }

        /// <summary>
        /// Gets a pointer to the elements of this array.
        /// </summary>
        /// <returns>A pointer to the allocated memory</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* GetUnsafePointer()
        {
            return _buffer;
        }

        /// <summary>
        /// Gets a string representation of the elements of this array.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents the elements of this array.
        /// </returns>
        public override string ToString()
        {
            if (_capacity == 0)
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
                        sb.Append(',');
                        sb.Append(' ');
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
        /// Gets an enumerator for iterate over the elements of this array.
        /// </summary>
        /// <returns>An enumerator over this array elements.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RefEnumerator<T> GetEnumerator()
        {
            return _buffer == null? default: new RefEnumerator<T>(_buffer, _capacity);
        }
    }

    /// <summary>
    /// Utilities for <see cref="NativeArray{T}"/>.
    /// </summary>
    public static partial class NativeArray
    {
        /// <summary>
        /// Creates a <see cref="NativeArray{T}"/> containing elements from start (inclusive) to end (exclusive)
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        /// <returns>An array with elements in the given range</returns>
        public static NativeArray<int> Range(int start, int end)
        {
            if (start == end)
                return default;

            if (start > end)
            {
                int length = start - end;
                NativeArray<int> array = new NativeArray<int>(length);

                for (int i = 0; start > end; start--, i++)
                {
                    array[i] = start;
                }

                return array;
            }
            else
            {
                int length = end - start;
                NativeArray<int> array = new NativeArray<int>(length);

                for (int i = 0; start < end; start++, i++)
                {
                    array[i] = start;
                }

                return array;
            }
        }

        /// <summary>
        /// Creates a <see cref="NativeArray{T}"/> containing elements from 0 to end (exclusive).
        /// </summary>
        /// <param name="end">The end value.</param>
        /// <returns>An array with elements in the given range</returns>
        public static NativeArray<int> Range(int end)
        {
            if (end < 0)
                throw new ArgumentOutOfRangeException("end", $"0 > {end}");

            return Range(0, end);
        }

        /// <summary>
        /// Creates a <see cref="NativeArray{T}"/> containing elements from start (inclusive) to end (inclusive)
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        /// <returns>An array with elements in the given range</returns>
        public static NativeArray<int> RangeClosed(int start, int end)
        {
            if (start == end)
                return default;

            if (start > end)
            {
                return Range(start, end - 1);
            }

            return Range(start, end + 1);
        }

        /// <summary>
        /// Creates a <see cref="NativeArray{T}"/> containing elements from 0 to end (inclusive).
        /// </summary>
        /// <param name="end">The end value.</param>
        /// <returns>An array with elements in the given range</returns>
        public static NativeArray<int> RangeClosed(int end)
        {
            if (end == 0)
                return default;

            if (end < 0)
                throw new ArgumentOutOfRangeException("end", $"0 > {end}");

            return Range(0, end + 1);
        }
    }
}
