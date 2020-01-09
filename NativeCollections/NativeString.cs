using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NativeCollections.Allocators;

namespace NativeCollections
{
    /// <summary>
    /// Represents a string allocated with unmanaged memory.
    /// </summary>
    /// <seealso cref="INativeContainer{char}" />
    /// <seealso cref="IDisposable" />
    /// <seealso cref="IEquatable{NativeString}" />
    /// <seealso cref="IEquatable{string}" />
    /// <seealso cref="IComparable{NativeString}" />
    /// <seealso cref="IComparable{string}" />
    unsafe public struct NativeString : INativeContainer<char>, IDisposable, IEquatable<NativeString>, IEquatable<string>, IComparable<NativeString>, IComparable<string>
    {
        private readonly char* _buffer;
        private readonly int _length;
        private readonly int _allocatorID;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NativeString"/> struct.
        /// </summary>
        /// <param name="str">The string to copy.</param>
        public NativeString(string str) : this(str, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeString"/> struct.
        /// </summary>
        /// <param name="str">The string to copy.</param>
        /// <param name="allocator">The allocator.</param>
        public NativeString(string str, Allocator allocator)
        {
            if (string.IsNullOrEmpty(str))
            {
                this = default;
            }
            else
            {
                fixed (char* p = str)
                {
                    int length = str.Length;
                    _buffer = allocator.Allocate<char>(length);
                    Unsafe.CopyBlockUnaligned(_buffer, p, (uint)(sizeof(char) * length));
                    this = new NativeString(_buffer, length, allocator);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeString"/> struct.
        /// </summary>
        /// <param name="span">The span to copy.</param>
        public NativeString(Span<char> span) : this(span, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeString"/> struct.
        /// </summary>
        /// <param name="span">The span to copy.</param>
        /// <param name="allocator">The allocator.</param>
        public NativeString(Span<char> span, Allocator allocator)
        {
            if (span.IsEmpty)
            {
                this = default;
            }
            else
            {
                char* p = (char*)Unsafe.AsPointer(ref span.GetPinnableReference());
                this = new NativeString(p, span.Length, allocator);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeString"/> struct.
        /// </summary>
        /// <param name="span">The span to copy.</param>
        public NativeString(ReadOnlySpan<char> span) : this(span, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeString"/> struct.
        /// </summary>
        /// <param name="span">The span to copy.</param>
        /// <param name="allocator">The allocator.</param>
        public NativeString(ReadOnlySpan<char> span, Allocator allocator)
        {
            if (span.IsEmpty)
            {
                this = default;
            }
            else
            {
                char* p = (char*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
                this = new NativeString(p, span.Length, allocator);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeString"/> struct.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="length">The length.</param>
        public NativeString(char* pointer, int length) : this(pointer, length, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeString"/> struct.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="length">The length.</param>
        /// <param name="allocator">The allocator.</param>
        /// <exception cref="ArgumentException">If the pointer is null, length is 0 or the allocator is not in cache.</exception>
        public NativeString(char* pointer, int length, Allocator allocator)
        {
            if(pointer == null && length == 0)
            {
                this = default;
                return;
            }

            if(pointer == null)
            {
                throw new ArgumentException("pointer is null");
            }

            if(length <= 0)
            {
                throw new ArgumentException("length cannot be negative or zero");
            }

            if(allocator.ID <= 0)
            {
                throw new ArgumentException("Allocator is not in cache");
            }

            _buffer = pointer;
            _length = length;
            _allocatorID = allocator.ID;
        }

        private NativeString(ref NativeString str)
        {
            if (!str.IsValid)
            {
                throw new ArgumentException("string is invalid");
            }

            Allocator allocator = str.GetAllocator()!;
            char* buffer = allocator.Allocate<char>(str._length);
            Unsafe.CopyBlockUnaligned(buffer, str._buffer, (uint)(sizeof(char) * str._length));

            _buffer = buffer;
            _length = str._length;
            _allocatorID = str._allocatorID;
        }
        #endregion

        /// <summary>
        /// Gets the length of this string.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length => _length;

        /// <summary>
        /// Checks if this string is allocated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid => _buffer != null;

        /// <summary>
        /// Gets a value indicating whether this string is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => _length == 0;

        /// <summary>
        /// Gets the allocator.
        /// </summary>
        /// <returns>The allocator used for this instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Allocator? GetAllocator()
        {
            return Allocator.GetAllocatorByID(_allocatorID);
        }

        /// <summary>
        /// Gets the <see cref="char"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="char"/> at the given index.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index is out of range.</exception>
        public readonly ref char this[int index]
        {
            get
            {
                if (index < 0 || index > _length)
                    throw new ArgumentOutOfRangeException(nameof(index), index.ToString());

                return ref _buffer[index];
            }
        }

        /// <summary>
        /// Copies the content of this string to the span.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <param name="index">Index of the destination.</param>
        /// <param name="count">The count.</param>
        /// <exception cref="ArgumentException">if the Span is empty</exception>
        /// <exception cref="InvalidOperationException">If NativeString is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the destinationIndex and/or count are out of range.</exception>
        public void CopyTo(in Span<char> span, int index, int count)
        {
            if (span.IsEmpty)
                throw new ArgumentException("Span is empty");

            if (_buffer == null)
                throw new InvalidOperationException("NativeString is invalid");

            if (index < 0 || index > span.Length)
                throw new ArgumentOutOfRangeException(nameof(index), index.ToString());

            if (count < 0 || count > _length || count > (span.Length - index))
                throw new ArgumentOutOfRangeException(nameof(count), count.ToString());

            char* pointer = (char*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
            void* src = _buffer;
            void* dst = pointer + index;
            Unsafe.CopyBlock(dst, src, (uint)(sizeof(char) * count));
        }

        /// <summary>
        /// Gets a <see cref="ReadOnlySpan{T}"/> using this string data.
        /// </summary>
        /// <returns>An span to this string data.</returns>
        public ReadOnlySpan<char> AsSpan()
        {
            return new ReadOnlySpan<char>(_buffer, _length);
        }

        /// <summary>
        /// Gets a string representation of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        /// <exception cref="InvalidOperationException">If NativeString is invalid</exception>
        public override string ToString()
        {
            if (_buffer == null)
                throw new InvalidOperationException("NativeString is invalid");

            return new string(_buffer, 0, _length);
        }

        /// <summary>
        /// Releases the resources used for this string.
        /// </summary>
        public void Dispose()
        {
            if (_buffer == null)
                return;

            Allocator.Default.Free(_buffer);
            this = default;
        }

        /// <summary>
        /// Gets an array using this instance values.
        /// </summary>
        /// <returns>A <see cref="char"/> array with this instance values.</returns>
        public char[] ToArray()
        {
            char[] array = new char[_length];
            CopyTo(array, 0, _length);
            return array;
        }

        /// <summary>
        /// Gets a <see cref="NativeArray{T}"> using this instance values.</see>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">If NativeString is invalid</exception>
        public NativeArray<char> ToNativeArray()
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeString is invalid");
            }

            NativeArray<char> array = new NativeArray<char>(_length, GetAllocator()!);
            Unsafe.CopyBlockUnaligned(array.GetUnsafePointer(), _buffer, (uint)(sizeof(char) * _length));
            return array;
        }

        /// <summary>
        /// Gets a <see cref="NativeArray{T}"> using this instance values and then disopose this instance.</see>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">If NativeString is invalid</exception>
        public NativeArray<char> ToNativeArrayAndDispose()
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeString is invalid");
            }

            // NativeArray now owns this NativeString memory
            NativeArray<char> array = new NativeArray<char>(_buffer, _length, GetAllocator()!);

            // No actual dispose, just invalidate this NativeString
            this = default;
            return array;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        ///   <see langword="true" /> if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, <see langword="false" />.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (obj is string s)
            {
                return Equals(s);
            }

            return obj is NativeString str && Equals(str);
        }

        /// <summary>
        /// Determines if this and the other <see cref="NativeString"/> are equals.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(NativeString other)
        {
            return AsSpan().SequenceEqual(other.AsSpan());
        }

        /// <summary>
        /// Compares this instance with other <see cref="NativeString"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This instance precedes <paramref name="other" /> in the sort order.
        /// Zero
        /// This instance occurs in the same position in the sort order as <paramref name="other" />.
        /// Greater than zero
        /// This instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        public int CompareTo(NativeString other, StringComparison comparisonType)
        {
            return AsSpan().CompareTo(other.AsSpan(), comparisonType);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This instance precedes <paramref name="other" /> in the sort order.
        /// Zero
        /// This instance occurs in the same position in the sort order as <paramref name="other" />.
        /// Greater than zero
        /// This instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        public int CompareTo(NativeString other)
        {
            return CompareTo(other, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (_buffer == null)
            {
                return 0;
            }

            int hash = 0;
            for(int i = 0; i < _length; ++i)
            {
                hash = HashCode.Combine(hash, _buffer[i]);
            }

            return hash;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals([AllowNull] string other)
        {
            return AsSpan().CompareTo(other.AsSpan(), StringComparison.CurrentCulture) == 0;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This instance precedes <paramref name="other" /> in the sort order.
        /// Zero
        /// This instance occurs in the same position in the sort order as <paramref name="other" />.
        /// Greater than zero
        /// This instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        public int CompareTo([AllowNull] string other)
        {
            return AsSpan().CompareTo(other.AsSpan(), StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Gets a deep clone of this instance.
        /// </summary>
        /// <returns>A copy of this instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeString Clone()
        {
            if (_buffer == null)
            {
                return default;
            }

            return new NativeString(ref this);
        }

        #region Operators
        public static implicit operator NativeString(string value)
        {
            return new NativeString(value);
        }

        public static implicit operator string(NativeString str)
        {
            return str.ToString();
        }

        public static bool operator ==(NativeString left, NativeString right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NativeString left, NativeString right)
        {
            return !(left == right);
        }

        public static bool operator ==(NativeString left, string right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NativeString left, string right)
        {
            return !(left == right);
        }

        public static bool operator ==(string left, NativeString right)
        {
            return right.Equals(left);
        }

        public static bool operator !=(string left, NativeString right)
        {
            return !(left == right);
        }
        #endregion
    }
}
