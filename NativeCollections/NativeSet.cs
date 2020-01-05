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
    /// Represents a collection of differents elements, and offer operations as union, difference, intersection and symmetric difference.
    /// </summary>
    /// <typeparam name="T">Type of elements.</typeparam>
    /// <seealso cref="NativeCollections.INativeContainer{T}" />
    /// <seealso cref="System.IDisposable" />
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeSetDebugView<>))]
    unsafe public struct NativeSet<T> : INativeContainer<T>, IDisposable where T: unmanaged
    {
        internal struct Entry
        {
            public T value;
            public int hashCode;
            public int next;
            public int bucket;
        }

        private Entry* _buffer;
        private int _capacity;
        private int _count;
        private int _freeCount;
        private int _freeList;

        private readonly int _allocatorID;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSet{T}"/> struct.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        public NativeSet(int initialCapacity) : this(initialCapacity, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSet{T}"/> struct.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        /// <param name="allocator">The allocator.</param>
        /// <exception cref="ArgumentException">If the capacity is negative or 0, or if the allocator is not in cache.</exception>
        public NativeSet(int initialCapacity, Allocator allocator)
        {
            if (initialCapacity <= 0)
            {
                throw new ArgumentException("initialCapacity should be greater than 0.", nameof(initialCapacity));
            }

            if (allocator.ID <= 0)
            {
                throw new ArgumentException("Allocator is not in cache.", "allocator");
            }

            _buffer = (Entry*)allocator.Allocate(initialCapacity, sizeof(Entry));
            _capacity = initialCapacity;
            _count = 0;
            _freeList = -1;
            _freeCount = 0;
            _allocatorID = allocator.ID;

            Initializate();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSet{T}"/> struct.
        /// </summary>
        /// <param name="elements">The initial elements.</param>
        public NativeSet(in Span<T> elements) : this(elements, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSet{T}"/> struct.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <param name="allocator">The allocator.</param>
        /// <exception cref="ArgumentException">If the capacity is negative or 0, or if the allocator is not in cache.</exception>
        public NativeSet(in Span<T> elements, Allocator allocator)
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
                _buffer = (Entry*)allocator.Allocate(elements.Length, sizeof(Entry));
                _capacity = elements.Length;
                _count = 0;
                _freeList = -1;
                _freeCount = 0;
                _allocatorID = allocator.ID;

                Initializate();

                foreach (var e in elements)
                {
                    Add(e);
                }
            }
        }

        /// <summary>
        /// Gets the number of elements in this set.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length => _count - _freeCount;

        /// <summary>
        /// Gets the number of elements this set can hold before resize.
        /// </summary>
        /// <value>
        /// The capacity.
        /// </value>
        public int Capacity => _capacity;

        /// <summary>
        /// Gets a value indicating whether this set have elements.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this set is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => _count == 0;

        /// <summary>
        /// Checks if this set is allocated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this set is valid; otherwise, <c>false</c>.
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
        /// Adds the specified value to the set.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value is not duplicated and was added, otherwise <c>false</c></returns>
        public bool Add(T value)
        {
            return AddIfAbsent(value);
        }

        /// <summary>
        /// Adds all the specified elements to the set.
        /// </summary>
        /// <param name="elements">The elements to add.</param>
        /// <returns>The number of elements added to the set.</returns>
        public int AddRange(in Span<T> elements)
        {
            if (elements.IsEmpty)
                return 0;

            int count = 0;
            foreach(var e in elements)
            {
                if(AddIfAbsent(e))
                {
                    ++count;
                }
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
            if (_count == 0)
                return false;

            var comparer = EqualityComparer<T>.Default;
            int hashCode = GetHash(value);
            int bucket = GetBucket(hashCode, _capacity);
            int index = _buffer[bucket].bucket;
            int last = -1;

            while(index >= 0)
            {
                ref Entry entry = ref _buffer[index];
                if (comparer.Equals(entry.value, value) && hashCode == entry.hashCode)
                {
                    if (last >= 0)
                    {
                        _buffer[last].next = _buffer[index].next;
                    }
                    else
                    {
                        _buffer[bucket].bucket = _buffer[bucket].next;
                    }

                    entry.hashCode = -1;
                    entry.next = _freeList;
                    _freeList = index;
                    _freeCount++;
                    return true;
                }

                last = index;
                index = entry.next;
            }

            return false;
        }

        /// <summary>
        /// Removes all the elements that meet the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The number of elements removed.</returns>
        public int RemoveIf(Predicate<T> predicate)
        {
            if (_buffer == null)
                return 0;

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
        /// Clears the content of this set.
        /// </summary>
        public void Clear()
        {
            if (_count == 0)
                return;

            Unsafe.InitBlockUnaligned(_buffer, 0, (uint)(sizeof(Entry) * _count));
            _count = 0;
            _freeCount = 0;
            _freeList = -1;

            Initializate();
        }

        /// <summary>
        /// Determines whether this set contains the value.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <returns>
        ///   <c>true</c> if the set contains the value; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T value)
        {
            if (_count == 0)
                return false;

            var comparer = EqualityComparer<T>.Default;
            int hashCode = GetHash(value);
            int bucket = GetBucket(hashCode, _capacity);
            int index = _buffer[bucket].bucket;

            while (index >= 0)
            {
                ref Entry entry = ref _buffer[index];
                if (comparer.Equals(entry.value, value) && hashCode == entry.hashCode)
                {
                    return true;
                }

                index = entry.next;
            }

            return false;
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
        /// Performs an union with this set and the specified elements.
        /// </summary>
        /// <param name="elements">The elements to perform the union with.</param>
        public void UnionWith(in Span<T> elements)
        {
            if (_buffer == null)
                return;

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
                return;

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
                return;

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
                return;

            foreach (ref var e in elements)
            {
                if (Contains(e))
                {
                    Remove(e);
                }
            }
        }

        /// <summary>
        /// Removes the excess spaces from this set.
        /// </summary>
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
            if (capacity < Length)
            {
                return;
            }

            Entry* newBuffer = (Entry*)GetAllocator()!.Allocate(capacity, sizeof(Entry));
            Unsafe.CopyBlock(newBuffer, _buffer, (uint)(Unsafe.SizeOf<Entry>() * _count));

            // Free old buffer
            Allocator.Default.Free(_buffer);

            for (int i = 0; i < capacity; i++)
            {
                newBuffer[i].bucket = -1;
            }

            int index = 0;
            int count = Length;

            for (int i = 0; i < count; i++)
            {
                ref Entry entry = ref newBuffer[i];
                int hashCode = GetHash(entry.value);

                if (hashCode >= 0)
                {
                    int bucket = GetBucket(hashCode, capacity);
                    newBuffer[index] = entry;
                    newBuffer[index].next = bucket;
                    newBuffer[bucket].bucket = index;
                    index++;
                }
            }

            _buffer = newBuffer;
            _freeCount = 0;
            _freeList = -1;
            _count = count;
            _capacity = capacity;
        }

        /// <summary>
        /// Ensures this set can hold the specified amount of elements before resize.
        /// </summary>
        /// <param name="capacity">The min capacity.</param>
        public void EnsureCapacity(int capacity)
        {
            if (capacity > _capacity)
            {
                Resize(capacity);
            }
        }

        /// <summary>
        /// Copies the content of this set to a <see cref="Span{T}" />.
        /// </summary>
        /// <param name="span">The destination span to copy the data.</param>
        /// <param name="destinationIndex">Start index of the destination where start to copy.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentException">Span is empty</exception>
        /// <exception cref="InvalidOperationException">Set is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the range provided by the destination index and count is invalid.</exception>
        public void CopyTo(in Span<T> span, int destinationIndex, int count)
        {
            if (span.IsEmpty)
                throw new ArgumentException("Span is empty");

            if (_buffer == null)
                throw new InvalidOperationException("NativeSet is invalid");

            if (destinationIndex < 0 || destinationIndex > span.Length)
                throw new ArgumentOutOfRangeException(nameof(destinationIndex), destinationIndex.ToString());

            if (count < 0 || count > _count || count > (span.Length - destinationIndex))
                throw new ArgumentOutOfRangeException(nameof(count), count.ToString());

            Enumerator enumerator = GetEnumerator();

            while (count > 0 && enumerator.MoveNext())
            {
                span[destinationIndex] = enumerator.Current;
                ++destinationIndex;
                --count;
            }
        }

        /// <summary>
        /// Allocates an array with the elements of this set.
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
        /// Creates a new <see cref="NativeArray{T}"/> with the elements of this set.
        /// </summary>
        /// <returns>A new array with the elements of this instance.</returns>
        public NativeArray<T> ToNativeArray()
        {
            if (_count == 0)
                return default;

            NativeArray<T> array = new NativeArray<T>(_count, GetAllocator()!);
            int i = 0;
            foreach(ref var e in this)
            {
                array[i] = e;
                ++i;
            }
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
        /// Gets a string representation of the elements of this set.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this set.
        /// </returns>
        public override string ToString()
        {
            if (_count == 0)
            {
                return "[]";
            }

            StringBuilder sb = StringBuilderCache.Acquire();
            Enumerator enumerator = GetEnumerator();
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
        /// Releases the resources used for this set.
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

        private void Initializate()
        {
            for(int i = 0; i < _capacity; ++i)
            {
                _buffer[i].bucket = -1;
            }
        }

        private bool AddIfAbsent(T value)
        {
            if (_buffer == null)
                return false; 

            var comparer = EqualityComparer<T>.Default;
            int hashCode = GetHash(value);
            int bucket = GetBucket(hashCode, _capacity);
            int index = _buffer[bucket].bucket;

            while(index >= 0)
            {
                ref Entry entry = ref _buffer[index];
                if (comparer.Equals(entry.value, value) && hashCode == entry.hashCode)
                {
                    return false;
                }

                index = entry.next;
            }

            if (_freeCount > 0)
            {
                index = _freeList;
                _freeList = _buffer[_freeList].next;
                _freeCount--;
            }
            else
            {
                if (_count == _capacity)
                {
                    Resize();
                    bucket = GetBucket(hashCode, _capacity);
                }

                index = _count;
                _count++;
            }

            _buffer[index].value = value;
            _buffer[index].hashCode = hashCode;
            _buffer[index].next = _buffer[bucket].bucket;
            _buffer[bucket].bucket = index;
            return true;
        }

        private void Resize()
        {
            Resize(_count * 2);
        }

        private void Resize(int newCapacity)
        {
            if (_buffer == null)
                return;

            Entry* newBuffer = GetAllocator()!.Allocate<Entry>(newCapacity);
            Unsafe.CopyBlock(newBuffer, _buffer, (uint)(sizeof(Entry) * _count));
            GetAllocator()!.Free(_buffer);

            for(int i = 0; i < newCapacity; i++)
            {
                newBuffer[i].bucket = -1;
            }

            for(int i = 0; i < _count; i++)
            {
                ref Entry entry = ref _buffer[i];
                if(entry.hashCode >= 0)
                {
                    int hashCode = GetHash(newBuffer[i].value);
                    int bucket = GetBucket(hashCode, newCapacity);
                    newBuffer[i].next = newBuffer[bucket].bucket;
                    newBuffer[bucket].bucket = i;
                }
            }

            _buffer = newBuffer;
            _capacity = newCapacity;
        }

        private readonly int GetHash(in T value)
        {
            return value.GetHashCode() & int.MaxValue;
        }

        private static int GetBucket(int hashCode, int capacity)
        {
            return hashCode % capacity;
        }

        /// <summary>
        /// Gets an enumerator over the elements of this set.
        /// </summary>
        /// <returns>An enumerator over the elements of this set.</returns>
        public Enumerator GetEnumerator()
        {
            Debug.Assert(_buffer != null);

            if (_buffer == null)
            {
                return default;
            }

            return new Enumerator(ref this);
        }

        /// <summary>
        /// An enumerator over the elements of a <see cref="NativeSet{T}"/>.
        /// </summary>
        public ref struct Enumerator
        {
            private Entry* _entries;
            private int _count;
            private int _index;

            /// <summary>
            /// Initializes a new instance of the <see cref="NativeSet{T}.Enumerator" /> struct.
            /// </summary>
            /// <param name="set">The set.</param>
            public Enumerator(ref NativeSet<T> set)
            {
                _entries = set._buffer;
                _count = set._count;
                _index = -1;
            }

            /// <summary>
            /// Gets a reference to the current element.
            /// </summary>
            public readonly ref T Current
            {
                get
                {
                    if (_index < 0 || _index > _count)
                        throw new ArgumentOutOfRangeException("index", _index.ToString());

                    return ref _entries[_index].value;
                }
            }

            /// <summary>
            /// Invalidates this enumerator.
            /// </summary>
            public void Dispose()
            {
                if (_entries == null)
                    return;

                this = default;
            }

            /// <summary>
            /// Moves to the next element.
            /// </summary>
            /// <returns><c>true</c> if moved to the next element.</returns>
            public bool MoveNext()
            {
                if (_count == 0)
                    return false;

                int i = _index + 1;
                while (i < _count)
                {
                    if (_entries[i].hashCode >= 0)
                    {
                        _index = i;
                        return true;
                    }

                    i++;
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
