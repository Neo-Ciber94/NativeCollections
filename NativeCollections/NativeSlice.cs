using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using NativeCollections.Utility;

namespace NativeCollections
{
    /// <summary>
    /// Represents a view to the elements of a native container.
    /// </summary>
    /// <typeparam name="T">Type of the elements.</typeparam>
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeSliceDebugView<>))]
    unsafe public readonly ref struct NativeSlice<T>
    {
        internal readonly void* _pointer;
        private readonly int _length;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSlice{T}"/> struct using the given array.
        /// </summary>
        /// <param name="array">The array.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSlice(T[] array) : this(array, 0, array.Length) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSlice{T}" /> struct using the given array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="length">The number of elements to include in the slice.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the length is out of range.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSlice(T[] array, int length) : this(array, 0, length) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSlice{T}" /> struct using the given array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The number of elements to include in the slice.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the startIndex or length are out of range.</exception>
        public NativeSlice(T[] array, int startIndex, int length)
        {
            if (startIndex < 0 || startIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex.ToString());

            if (length < 0 || length > (array.Length - startIndex))
                throw new ArgumentOutOfRangeException(nameof(length), length.ToString());

            _pointer = Unsafe.AsPointer(ref array[startIndex]);
            _length = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSlice{T}" /> struct.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="length">The length.</param>
        public NativeSlice(void* pointer, int length)
        {
            if (pointer == null)
                throw new ArgumentException("Invalid pointer");

            if (length < 0)
                throw new ArgumentException($"Invalid length: {length}");

            _pointer = pointer;
            _length = length;
        }

        /// <summary>
        /// Gets the number of elements in this range.
        /// </summary>
        public int Length => _length;

        /// <summary>
        /// Determines if this slice have elements.
        /// </summary>
        public bool IsEmpty => _length == 0;

        /// <summary>
        /// Gets a <see cref="Span{T}"/> from this slice.
        /// </summary>
        public Span<T> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Span<T>(_pointer, _length);
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="T" /> element at the given index.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>A reference to the element at the given index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the range is out of bounds.</exception>
        public ref T this[int index]
        {
            get
            {
                if (index < 0 || index > _length)
                    throw new ArgumentOutOfRangeException("index", $"{index}");

                ref T pointer = ref Unsafe.AsRef<T>(_pointer);
                return ref Unsafe.Add(ref pointer, index);
            }
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="T" /> element at the given index.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>A reference to the element at the given index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the range is out of bounds.</exception>
        public ref T this[Index index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                int i = index.IsFromEnd ? _length - index.Value - 1 : index.Value;
                return ref this[i];
            }
        }

        /// <summary>
        /// Gets the elements in the specified range of this slice.
        /// </summary>
        /// <value>
        /// The <see cref="NativeCollections.NativeSlice{T}" />.
        /// </value>
        /// <param name="range">The range.</param>
        /// <returns>A slice within the given range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the range is out of bounds.</exception>
        public NativeSlice<T> this[Range range]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var (index, length) = range.GetOffsetAndLength(_length);
                return Slice(index, length);
            }
        }

        /// <summary>
        /// Copies the elements in this slice to the given span.
        /// </summary>
        /// <param name="span">The destination span.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentException">If the span is empty.</exception>
        /// <exception cref="InvalidOperationException">If the slice is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the count are out of range.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(in Span<T> span)
        {
            CopyTo(span, 0, _length);
        }

        /// <summary>
        /// Copies the elements in this slice to the given span.
        /// </summary>
        /// <param name="span">The destination span.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentException">If the span is empty.</exception>
        /// <exception cref="InvalidOperationException">If the slice is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the count are out of range.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(in Span<T> span, int count)
        {
            CopyTo(span, 0, count);
        }

        /// <summary>
        /// Copies the elements in this slice to the given span.
        /// </summary>
        /// <param name="span">The destination span.</param>
        /// <param name="index">The start index of the copy within the span.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentException">If the span is empty.</exception>
        /// <exception cref="InvalidOperationException">If the slice is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the index and/or count are out of range.</exception>
        public void CopyTo(in Span<T> span, int index, int count)
        {
            if (span.IsEmpty)
                throw new ArgumentException("Span is empty");

            if (_pointer == null)
                throw new InvalidOperationException("NativeSlice is invalid");

            if (index < 0 || index > span.Length)
                throw new ArgumentOutOfRangeException(nameof(index), index.ToString());

            if (count < 0 || count > _length || count > (span.Length - index))
                throw new ArgumentOutOfRangeException(nameof(count), count.ToString());

            void* dst = Unsafe.AsPointer(ref span[index]);
            Unsafe.CopyBlockUnaligned(dst, _pointer, (uint)(Unsafe.SizeOf<T>() * count));
        }

        /// <summary>
        /// Allocates a new array with the elements of this slice.
        /// </summary>
        /// <returns>A newly allocated array with the elements of this slice.</returns>
        public T[] ToArray()
        {
            if (_length == 0)
            {
                return Array.Empty<T>();
            }

            T[] array = new T[_length];
            CopyTo(array);
            return array;
        }

        /// <summary>
        /// Gets a range within this slice.
        /// </summary>
        /// <param name="index">The start index.</param>
        /// <returns>A slice from this instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSlice<T> Slice(int index)
        {
            return Slice(index, _length - index);
        }

        /// <summary>
        /// Gets a range within this slice.
        /// </summary>
        /// <param name="index">The start index.</param>
        /// <param name="length">The number of elements.</param>
        /// <returns>A slice from this instance.</returns>
        public NativeSlice<T> Slice(int index, int length)
        {
            Debug.Assert(_pointer != null);

            if (index < 0 || index > _length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index.ToString());
            }

            if (length < 0 || length > (_length - index))
            {
                throw new ArgumentOutOfRangeException(nameof(length), length.ToString());
            }

            byte* p = (byte*)_pointer + (Unsafe.SizeOf<T>() * index);
            return new NativeSlice<T>(p, length);
        }

        /// <summary>
        /// Gets a string representation of the elements of this array.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents the elements of this array.
        /// </returns>
        public override string ToString()
        {
            if (_length == 0)
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
                    sb.Append(value?.ToString()?? string.Empty);

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
        public static implicit operator NativeSlice<T>(T[] array)
        {
            return new NativeSlice<T>(array);
        }

        /// <summary>
        /// Gets an enumerator for iterate over the elements of this slice.
        /// </summary>
        /// <returns>An enumerator over this array elements.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RefEnumerator<T> GetEnumerator()
        {
            Debug.Assert(_pointer != null);

            if(_pointer == null)
            {
                return default;
            }

            return new RefEnumerator<T>(_pointer, _length);
        }
    }
}
