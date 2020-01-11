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
    /// Represets an ordered collection of different elements.
    /// </summary>
    /// <typeparam name="T">Type of the elements.</typeparam>
    /// <seealso cref="INativeContainer{T}" />
    /// <seealso cref="IDisposable" />
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeSortedSetDebugView<>))]
    unsafe public struct NativeSortedSet<T> : INativeContainer<T>, IDisposable where T: unmanaged
    {
        private T* _buffer;
        private int _count;
        private int _capacity;
        private int _allocatorID;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSortedSet{T}"/> struct.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        public NativeSortedSet(int initialCapacity) : this(initialCapacity, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSortedSet{T}"/> struct.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        /// <param name="allocator">The allocator.</param>
        /// <exception cref="System.ArgumentException">If the initialCapacity is negative or zero, or the allocator is not in cache.</exception>
        public NativeSortedSet(int initialCapacity, Allocator allocator)
        {
            if(initialCapacity <= 0)
            {
                throw new ArgumentException("initialCapacity cannot be negative or zero", nameof(initialCapacity));
            }

            if(Allocator.IsCached(allocator) is false)
            {
                throw new ArgumentException("allocator is not in cache", nameof(allocator));
            }

            _buffer = allocator.Allocate<T>(initialCapacity);
            _capacity = initialCapacity;
            _allocatorID = allocator.ID;
            _count = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSortedSet{T}"/> struct.
        /// </summary>
        /// <param name="elements">The initial elements.</param>
        /// <param name="allocator">The allocator.</param>
        /// <exception cref="InvalidOperationException">Span is empty</exception>
        public NativeSortedSet(in Span<T> elements) : this(elements, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSortedSet{T}"/> struct.
        /// </summary>
        /// <param name="elements">The initial elements.</param>
        /// <param name="allocator">The allocator.</param>
        /// <exception cref="InvalidOperationException">Span is empty</exception>
        /// <exception cref="ArgumentException">If elements is empty or the allocator is not in cache.</exception>
        public NativeSortedSet(in Span<T> elements, Allocator allocator)
        {
            if (elements.IsEmpty)
            {
                throw new InvalidOperationException("Span is empty");
            }

            if (Allocator.IsCached(allocator) is false)
            {
                throw new ArgumentException("allocator is not in cache", nameof(allocator));
            }

            int length = elements.Length;
            _buffer = allocator.Allocate<T>(length);
            _capacity = length;
            _allocatorID = allocator.ID;
            _count = 0;

            foreach(ref var e in elements)
            {
                Add(e);
            }
        }

        internal NativeSortedSet(void* pointer, int length, Allocator allocator)
        {
            Debug.Assert(pointer != null);
            Debug.Assert(length > 0);
            Debug.Assert(Allocator.IsCached(allocator));

            _buffer = (T*)pointer;
            _count = length;
            _capacity = length;
            _allocatorID = allocator.ID;
        }

        /// <summary>
        /// Gets the number of elements this set can hold before resize.
        /// </summary>
        /// <value>
        /// The capacity.
        /// </value>
        public int Capacity => _capacity;

        /// <summary>
        /// Gets the number of elements in this set.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length => _count;

        /// <summary>
        /// Checks if this set is allocated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid => _buffer != null;

        /// <summary>
        /// Gets a value indicating whether this set have elements.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => _count == 0;

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <value>
        /// The element.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>The value at the given index.</returns>
        public readonly ref T this[int index]
        {
            get
            {
                if (_buffer == null)
                {
                    throw new InvalidOperationException("NativeSortedSet is invalid");
                }

                if (index < 0 || index >= _count)
                {
                    throw new ArgumentOutOfRangeException("index is out of range", nameof(index));
                }

                return ref _buffer[index];
            }
        }

        /// <summary>
        /// Gets the element with minimun value in the set.
        /// </summary>
        public readonly ref T Min
        {
            get
            {
                if (_buffer == null)
                {
                    throw new InvalidOperationException("NativeSortedSet is invalid");
                }

                if(_count == 0)
                {
                    throw new InvalidOperationException("NativeSortedSet is empty");
                }

                return ref _buffer[0];
            }
        }

        /// <summary>
        /// Gets the element with maximun value in the set.
        /// </summary>
        public readonly ref T Max
        {
            get
            {
                if (_buffer == null)
                {
                    throw new InvalidOperationException("NativeSortedSet is invalid");
                }

                if (_count == 0)
                {
                    throw new InvalidOperationException("NativeSortedSet is empty");
                }

                return ref _buffer[_count - 1];
            }
        }

        /// <summary>
        /// Gets the allocator.
        /// </summary>
        /// <returns>The allocator used for this set.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Allocator? GetAllocator()
        {
            return Allocator.GetAllocatorByID(_allocatorID);
        }

        /// <summary>
        /// Adds the specified value to the set.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value was added, otherwise <c>false</c>.</returns>
        public bool Add(T value)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedSet is invalid");
            }

            return AddIfAbsent(value);
        }

        /// <summary>
        /// Adds all the given values to the set.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <returns>The number of elements added to the set.</returns>
        public int AddAll(in Span<T> elements)
        {
            int count = 0;
            for (int i = 0; i < elements.Length; ++i)
            {
                Add(elements[i]);
                count++;
            }
            return count;
        }

        /// <summary>
        /// Removes the specified value from the set.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value was removed.</returns>
        public bool Remove(T value)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedSet is invalid");
            }

            int index = BinarySearch(value);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the 0-index value in the set.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedSet is invalid");
            }

            if (index < 0 || index > _count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index.ToString());
            }

            _count--;
            if (_count > index)
            {
                int length = _count - index;
                T* src = _buffer + index + 1;
                T* dst = _buffer + index;
                Unsafe.CopyBlock(dst, src, (uint)(sizeof(T) * length));
            }

            _buffer[_count] = default;
        }

        /// <summary>
        /// Removes all the elements that meet the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The number of elements removed.</returns>
        public int RemoveIf(Predicate<T> predicate)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedSet is invalid");
            }

            int count = 0;
            foreach (ref var e in this)
            {
                if (predicate(e))
                {
                    Remove(e);
                    ++count;
                }
            }

            return count;
        }

        /// <summary>
        /// Performs an union with this set and the specified elements.
        /// </summary>
        /// <param name="elements">The elements to perform the union with.</param>
        public void UnionWith(in Span<T> elements)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedSet is invalid");
            }

            foreach (ref var e in elements)
            {
                Add(e);
            }
        }

        /// <summary>
        /// Performs an intersection with this set and the specified elements.
        /// </summary>
        /// <param name="elements">The elements to perform the intersection with.</param>
        public void IntersectionWith(in Span<T> elements)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedSet is invalid");
            }

            using NativeArray<T> copy = ToNativeArray();

            foreach (ref var e in copy)
            {
                if (!SpanHelper.Contains(elements, e))
                {
                    Remove(e);
                }
            }
        }

        /// <summary>
        /// Performs a difference with this set and the specified elements.
        /// </summary>
        /// <param name="elements">The elements to perform the difference with.</param>
        public void DifferenceWith(in Span<T> elements)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedSet is invalid");
            }

            foreach (ref var e in elements)
            {
                Remove(e);
            }
        }

        /// <summary>
        /// Performs a symmetric difference with this set and the specified elements.
        /// </summary>
        /// <param name="elements">The elements to perform the symmetric difference.</param>
        public void SymmetricDifferenceWith(in Span<T> elements)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedSet is invalid");
            }

            foreach (ref var e in elements)
            {
                if (Contains(e))
                {
                    Remove(e);
                }
            }
        }

        /// <summary>
        /// Clears the contents of this set.
        /// </summary>
        public void Clear()
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedSet is invalid");
            }

            if (_count == 0)
            {
                return;
            }

            _count = 0;
        }

        /// <summary>
        /// Determines whether this set contains the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if set contains the value; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T value)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedSet is invalid");
            }

            return BinarySearch(value) >= 0;
        }

        /// <summary>
        /// Determines whether this set elements contains all the values.
        /// </summary>
        /// <param name="elements">The elements to locate.</param>
        /// <returns>
        ///   <c>true</c> if the set contains all the values; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsAll(in Span<T> elements)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedSet is invalid");
            }

            foreach (ref var e in elements)
            {
                if (!Contains(e))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a <see cref="NativeSortedSet{T}"/> within values in the given range.
        /// </summary>
        /// <param name="lowerValue">The lower value.</param>
        /// <param name="upperValue">The upper value.</param>
        /// <returns>A set with the values in the given range.</returns>
        public NativeSortedSet<T> GetRange(T lowerValue, T upperValue)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedSet is invalid");
            }

            int startIndex = BinarySearch(lowerValue);
            int toIndex = BinarySearch(upperValue);

            if (startIndex < 0)
            {
                startIndex = ~startIndex;
            }

            if (toIndex < 0)
            {
                toIndex = ~toIndex;
            }

            if (toIndex >= _count)
            {
                toIndex = _count - 1;
            }

            if (startIndex >= _count)
            {
                startIndex = _count - 1;
            }

            if (startIndex == toIndex)
            {
                return default;
            }

            if (startIndex > toIndex)
            {
                throw new ArgumentException($"lowerValue is greater than toKey: {lowerValue} > {upperValue}");
            }

            Allocator allocator = GetAllocator()!;
            int length = toIndex - startIndex + 1;
            T* buffer = allocator.Allocate<T>(length);
            Unsafe.CopyBlockUnaligned(buffer, _buffer + startIndex, (uint)(sizeof(T) * length));
            return new NativeSortedSet<T>(buffer, length, allocator);
        }

        /// <summary>
        /// Gets a <see cref="NativeSortedSet{T}"/> within values in the given range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns>A set with the values in the given range.</returns>
        public NativeSortedSet<T> GetRange(Range range)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            var (startIndex, length) = range.GetOffsetAndLength(_count);

            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException($"start index cannot be negative: {range}", nameof(range));
            }

            if (length == 0 || startIndex >= _count)
            {
                return default;
            }

            Allocator allocator = GetAllocator()!;
            T* buffer = allocator.Allocate<T>(length);
            Unsafe.CopyBlockUnaligned(buffer, _buffer + startIndex, (uint)(sizeof(T) * length));
            return new NativeSortedSet<T>(buffer, length, allocator);
        }

        /// <summary>
        /// Removes the excess spaces from this set.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TrimExcess()
        {
            TrimExcess(Length);
        }

        /// <summary>
        /// Removes the excess spaces from this set.
        /// </summary>
        /// <param name="capacity">The min capacity.</param>
        public void TrimExcess(int capacity)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedSet is invalid");
            }

            if (capacity < Length)
            {
                return;
            }

            Resize(capacity);
        }

        /// <summary>
        /// Ensures this set can hold the specified amount of elements before resize.
        /// </summary>
        /// <param name="capacity">The min capacity.</param>
        public void EnsureCapacity(int capacity)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedSet is invalid");
            }

            if (capacity > _capacity)
            {
                Resize(capacity);
            }
        }

        /// <summary>
        /// Copies the content of this set to a <see cref="Span{T}" />.</summary>
        /// <param name="span">The destination span to copy the data.</param>
        /// <param name="index">Start index of the destination where start to copy.</param>
        /// <param name="count">The number of elements to copy.</param>
        public readonly void CopyTo(in Span<T> span, int index, int count)
        {
            if (span.IsEmpty)
                throw new ArgumentException("Span is empty");

            if (_buffer == null)
                throw new InvalidOperationException("NativeSortedSet is invalid");

            if (index < 0 || index > span.Length)
                throw new ArgumentOutOfRangeException(nameof(index), index.ToString());

            if (count < 0 || count > _count || count > (span.Length - index))
                throw new ArgumentOutOfRangeException(nameof(count), count.ToString());

            void* dst = (T*)Unsafe.AsPointer(ref span.GetPinnableReference()) + index;
            void* src = _buffer;
            Unsafe.CopyBlock(dst, src, (uint)(sizeof(T) * count));
        }

        /// <summary>
        /// Allocates a new array with the elements of this set.
        /// </summary>
        /// <returns>A newly created array with this set elements.</returns>
        public T[] ToArray()
        {
            if(_count == 0)
            {
                return Array.Empty<T>();
            }

            T[] array = new T[_count];
            CopyTo(array, 0, _count);
            return array;
        }

        /// <summary>
        /// Allocates a <see cref="NativeArray{T}"/> with the elements of this set.
        /// </summary>
        /// <returns>A newly create array with this set elements.</returns>
        public NativeArray<T> ToNativeArray()
        {
            if (_count == 0)
            {
                return default;
            }

            Allocator allocator = GetAllocator()!;
            NativeArray<T> array = new NativeArray<T>(_count, allocator);
            Unsafe.CopyBlockUnaligned(array.GetUnsafePointer(), _buffer, (uint)(sizeof(T) * _count));
            return array;
        }

        /// <summary>
        /// Creates a new <see cref="NativeArray{T}"/> with the elements of this set and dispose this set.
        /// </summary>
        /// <param name="createNewArrayIfNeeded">If <c>true</c> a new array will be created if the capacity of this
        /// set is different than its length; otherwise is guaranteed the new array will use this set memory.</param>
        /// <returns>A newly created array with this list elements.</returns>
        public NativeArray<T> ToNativeArrayAndDispose(bool createNewArrayIfNeeded = true)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedSet is invalid");
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
        /// Releases the resources of this set.
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
        /// Gets a string representation of the elements of this set.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this set.
        /// </returns>
        public override string ToString()
        {
            if (_count == 0)
            {
                return "[]";
            }

            StringBuilder sb = StringBuilderCache.Acquire();
            RefEnumerator<T> enumerator = GetEnumerator();
            sb.Append('[');

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
        /// Gets an enumerator over the elements of this set.
        /// </summary>
        /// <returns>An enumerator over this set elements.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RefEnumerator<T> GetEnumerator()
        {
            return _buffer == null ? default : new RefEnumerator<T>(_buffer, _count);
        }

        private bool AddIfAbsent(T value)
        {
            if(_count == 0)
            {
                _buffer[_count++] = value;
                return true;
            }

            if(_count == _capacity)
            {
                RequireCapacity(_count + 1);
            }

            int index = BinarySearch(value);
            if(index < 0)
            {
                index = ~index;

                int length = _count - index;
                T* src = _buffer + index;
                T* dst = src + 1;
                Unsafe.CopyBlockUnaligned(dst, src, (uint)(sizeof(T) * length));
                *src = value;
                _count++;
                return true;
            }

            return false;
        }

        private void RequireCapacity(int min)
        {
            if(min > _capacity)
            {
                int length = _capacity * 2;
                if (min < length)
                {
                    Resize(length);
                }
                else
                {
                    Resize(min);
                }
            }
        }

        private void Resize(int capacity)
        {
            Debug.Assert(_buffer != null);
            _buffer = GetAllocator()!.Reallocate<T>(_buffer, capacity);
            _capacity = capacity;
        }

        private int BinarySearch(T value)
        {
            int start = 0;
            int end = _count - 1;

            var comparer = Comparer<T>.Default;

            while (start <= end)
            {
                int mid = start + ((end - start) >> 1);
                int comp = comparer.Compare(_buffer[mid], value);

                if (comp == 0)
                {
                    return mid;
                }
                if (comp < 0)
                {
                    start = mid + 1;
                }
                else
                {
                    end = mid - 1;
                }
            }

            return ~start;
        }
    }
}
