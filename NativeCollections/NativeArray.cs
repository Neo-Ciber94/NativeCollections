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
    /// <seealso cref="System.IDisposable" />
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeArrayDebugView<>))]
    unsafe public struct NativeArray<T> : INativeContainer<T>, IDisposable where T : unmanaged
    {
        /// <summary>
        /// Exposes methods for iterate over the contents of a <see cref="NativeArray{T}"/>.
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
            /// <param name="array">The array.</param>
            public Enumerator(ref NativeArray<T> array)
            {
                _pointer = array._buffer;
                _length = array._capacity;
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

        internal void* _buffer;
        private int _capacity;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeArray{T}" /> struct.
        /// </summary>
        /// <param name="capacity">The capacity of the array.</param>
        public NativeArray(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException($"capacity must be greater than 0: {capacity}");

            _buffer = Allocator.Default.Allocate(sizeof(T) * capacity);
            _capacity = capacity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeArray{T}" /> struct by copying the specify elements.
        /// </summary>
        /// <param name="elements">The elements to initializate the array.</param>
        public NativeArray(Span<T> elements)
        {
            if (elements.IsEmpty)
            {
                this = default;
            }
            else
            {
                _buffer = Allocator.Default.Allocate(sizeof(T) * elements.Length);
                _capacity = elements.Length;

                void* source = Unsafe.AsPointer(ref MemoryMarshal.GetReference(elements));
                Unsafe.CopyBlock(_buffer, source, (uint)(sizeof(T) * _capacity));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeArray{T}" /> struct by using the given pointer.
        /// </summary>
        /// <param name="pointer">The pointer to the elements.</param>
        /// <param name="length">The number of elements in the pointer.</param>
        public NativeArray(void* pointer, int length)
        {
            if (pointer == null)
                throw new ArgumentException("Invalid pointer");

            if (length <= 0)
                throw new ArgumentException($"Invalid length: {length}", nameof(length));

            _buffer = pointer;
            _capacity = length;
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
                if (index < 0 || index > _capacity)
                    throw new ArgumentOutOfRangeException("index", $"{index}");

                ref T pointer = ref Unsafe.AsRef<T>(_buffer);
                return ref Unsafe.Add(ref pointer, index);
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

                if (i < 0 || i > _capacity)
                    throw new ArgumentOutOfRangeException("index", $"{index}");

                ref T pointer = ref Unsafe.AsRef<T>(_buffer);
                return ref Unsafe.Add(ref pointer, i);
            }
        }

        /// <summary>
        /// Fills the content of this array with the given value.
        /// </summary>
        /// <param name="value">The value to use.</param>
        public void Fill(T value)
        {
            if (_capacity == 0)
                return;

            if (sizeof(T) == 1)
            {
                ref byte value2 = ref Unsafe.As<T, byte>(ref value);
                Unsafe.InitBlock(_buffer, value2, (uint)_capacity);
            }
            else
            {
                T* pointer = (T*)_buffer;
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

                for (; count > 0; count--)
                {
                    pointer[pos++] = value;
                }
            }
        }

        /// <summary>
        /// Reverses the order of the content of this array.
        /// </summary>
        public void Reverse()
        {
            Reverse(0, _capacity - 1);
        }

        /// <summary>
        /// Reverses the order of the content of this array.
        /// </summary>
        /// <param name="start">The start index.</param>
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

            NativeCollectionUtilities.Reverse<T>(_buffer, start, end);
        }

        /// <summary>
        /// Get the index of the specified value.
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <returns>The index of the element or -1 if not found.</returns>
        public int IndexOf(T value)
        {
            return IndexOf(value, 0, _capacity - 1);
        }

        /// <summary>
        /// Get the index of the specified value.
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <param name="start">The start index of the search.</param>
        /// <returns>The index of the element or -1 if not found.</returns>
        public int IndexOf(T value, int start)
        {
            return IndexOf(value, start, _capacity - 1);
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

            return NativeCollectionUtilities.IndexOf(_buffer, start, end, value);
        }

        /// <summary>
        /// Determines whether this array contains the specified value.
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <returns>
        ///   <c>true</c> if the array contains the value; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T value)
        {
            return IndexOf(value) >= 0;
        }

        /// <summary>
        /// Get the index of the last specified value.
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <returns>The index of the element or -1 if not found.</returns>
        public int LastIndexOf(T value)
        {
            return LastIndexOf(value, 0, _capacity - 1);
        }

        /// <summary>
        /// Get the index of the last specified value.
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <param name="start">The start index of the search.</param>
        /// <returns>The index of the element or -1 if not found.</returns>
        public int LastIndexOf(T value, int start)
        {
            return LastIndexOf(value, start, _capacity - 1);
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

            return NativeCollectionUtilities.LastIndexOf(_buffer, start, end, value);
        }

        /// <summary>
        /// Copies the content of this array into the specified array.
        /// </summary>
        /// <param name="span">The span to copy the elements.</param>
        public readonly void CopyTo(in Span<T> span)
        {
            CopyTo(span, 0, 0, _capacity);
        }

        /// <summary>
        /// Copies the content of this array into the specified array.
        /// </summary>
        /// <param name="span">The span to copy the elements.</param>
        /// <param name="count">The number of elements to copy.</param>
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

            int size = _capacity - sourceIndex;

            if (count < 0 || count > size || (span.Length - destinationIndex) < size)
                throw new ArgumentException(nameof(count), count.ToString());

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
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeArray is invalid");
            }

            T[] array = new T[_capacity];
            CopyTo(array);
            return array;
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
        /// Releases the allocated memory of this array.
        /// </summary>
        public void Dispose()
        {
            if (_buffer == null)
                return;

            Allocator.Default.Free(_buffer);
            _buffer = null;
            _capacity = 0;
        }

        /// <summary>
        /// Gets a pointer to the elements of this array.
        /// </summary>
        /// <returns>A pointer to the allocated memory</returns>
        public void* GetUnsafePointer()
        {
            return _buffer;
        }

        /// <summary>
        /// Gets an enumerator for iterate over the elements of this array.
        /// </summary>
        /// <returns>An enumerator over this array elements.</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(ref this);
        }
    }

    /// <summary>
    /// Utilities for <see cref="NativeArray{T}"/>.
    /// </summary>
    public static class NativeArray
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
