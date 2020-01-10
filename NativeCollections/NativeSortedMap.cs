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
    /// Represents a collection of key-values sorted by key where each key is associated to a value.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="NativeCollections.INativeContainer{System.Collections.Generic.KeyValuePair{TKey, TValue}}" />
    /// <seealso cref="System.IDisposable" />
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeSortedMapDebugView<,>))]
    unsafe public struct NativeSortedMap<TKey, TValue> : INativeContainer<KeyValuePair<TKey, TValue>>, IDisposable where TKey : unmanaged where TValue : unmanaged
    {
        internal struct Entry
        {
            public TKey key;
            public TValue value;

            public Entry(TKey key, TValue value)
            {
                this.key = key;
                this.value = value;
            }
        }

        /// <summary>
        /// A collection of the keys of a <see cref="NativeSortedMap{TKey, TValue}"/>.
        /// </summary>
        public ref struct KeyCollection
        {
            private NativeSortedMap<TKey, TValue> _map;

            internal KeyCollection(ref NativeSortedMap<TKey, TValue> map)
            {
                if(map.IsValid is false)
                {
                    throw new InvalidOperationException("NativeSortedMap is invalid");
                }

                _map = map;
            }

            /// <summary>
            /// Gets the number of keys.
            /// </summary>
            /// <value>
            /// The length.
            /// </value>
            public readonly int Length => _map.Length;

            /// <summary>
            /// Determines whether the specified key exists in this collection.
            /// </summary>
            /// <param name="key">The key to locate.</param>
            /// <returns>
            ///   <c>true</c> if the key exists; otherwise, <c>false</c>.
            /// </returns>
            public readonly bool Contains(TKey key) => _map.ContainsKey(key);

            /// <summary>
            /// Copies the keys to the specified span.
            /// </summary>
            /// <param name="span">The span.</param>
            /// <param name="startIndex">The start index.</param>
            /// <param name="count">The count.</param>
            /// <exception cref="ArgumentException">If the span is empty.</exception>
            /// <exception cref="ArgumentOutOfRangeException">If arguments <c>startIndex</c> or <c>count</c> are out of range.</exception>
            public readonly void CopyTo(in Span<TKey> span, int startIndex, int count)
            {
                if (span.IsEmpty)
                    throw new ArgumentException("Span is empty", nameof(span));

                if (startIndex < 0 || startIndex > span.Length)
                    throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex.ToString());

                if (count < 0 || count > Length)
                    throw new ArgumentOutOfRangeException(nameof(count), count.ToString());

                for (int i = 0; count > 0; --count, ++i)
                {
                    ref Entry entry = ref _map._buffer[i];
                    span[startIndex++] = entry.key;
                }
            }

            /// <summary>
            /// Gets an enumerator over the keys.
            /// </summary>
            /// <returns>An enumerator over the keys.</returns>
            public Enumerator GetEnumerator()
            {
                return new Enumerator(ref _map);
            }

            /// <summary>
            /// An enumerator over the keys.
            /// </summary>
            public ref struct Enumerator
            {
                private readonly Entry* _entries;
                private readonly int _count;
                private int _index;

                internal Enumerator(ref NativeSortedMap<TKey, TValue> map)
                {
                    _entries = map._buffer;
                    _count = map._count;
                    _index = -1;
                }

                /// <summary>
                /// Gets a reference to the current key.
                /// </summary>
                public readonly ref TKey Current
                {
                    get
                    {
                        if (_index < 0 || _index > _count)
                            throw new ArgumentOutOfRangeException("index", _index.ToString());

                        return ref _entries[_index].key;
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
                /// Moves to the next key.
                /// </summary>
                /// <returns><c>true</c> if moved to the next key.</returns>
                public bool MoveNext()
                {
                    if (_count == 0)
                        return false;

                    int i = _index + 1;
                    while (i < _count)
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

        /// <summary>
        /// A collection of the values of a <see cref="NativeMap{TKey, TValue}"/>.
        /// </summary>
        public ref struct ValueCollection
        {
            private NativeSortedMap<TKey, TValue> _map;

            internal ValueCollection(ref NativeSortedMap<TKey, TValue> map)
            {
                if (map.IsValid is false)
                {
                    throw new InvalidOperationException("NativeSortedMap is invalid");
                }

                _map = map;
            }

            /// <summary>
            /// Gets the number of values.
            /// </summary>
            /// <value>
            /// The length.
            /// </value>
            public readonly int Length => _map.Length;

            /// <summary>
            /// Determines whether the specified value exists in this collection.
            /// </summary>
            /// <param name="value">The value to locate.</param>
            /// <returns>
            ///   <c>true</c> if the value exists; otherwise, <c>false</c>.
            /// </returns>
            public readonly bool Contains(TValue value) => _map.ContainsValue(value);

            /// <summary>
            /// Copies the values to the specified span.
            /// </summary>
            /// <param name="span">The span.</param>
            /// <param name="startIndex">The start index.</param>
            /// <param name="count">The count.</param>
            /// <exception cref="ArgumentException">If the span is empty.</exception>
            /// <exception cref="ArgumentOutOfRangeException">If arguments <c>startIndex</c> or <c>count</c> are out of range.</exception>
            public readonly void CopyTo(in Span<TValue> span, int startIndex, int count)
            {
                if (span.IsEmpty)
                    throw new ArgumentException("Span is empty", nameof(span));

                if (startIndex < 0 || startIndex > span.Length)
                    throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex.ToString());

                if (count < 0 || count > Length)
                    throw new ArgumentException(nameof(count), count.ToString());

                for (int i = 0; count > 0; --count, ++i)
                {
                    ref Entry entry = ref _map._buffer[i];
                    span[startIndex++] = entry.value;
                }
            }

            /// <summary>
            /// Gets an enumerator over the values.
            /// </summary>
            /// <returns>An enumerator over the values.</returns>
            public Enumerator GetEnumerator()
            {
                return new Enumerator(ref _map);
            }

            /// <summary>
            /// An enumerator over the values.
            /// </summary>
            public ref struct Enumerator
            {
                private readonly Entry* _entries;
                private readonly int _count;
                private int _index;

                internal Enumerator(ref NativeSortedMap<TKey, TValue> map)
                {
                    _entries = map._buffer;
                    _count = map._count;
                    _index = -1;
                }

                /// <summary>
                /// Gets a reference to the current value.
                /// </summary>
                public ref TValue Current
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
                /// Moves to the next value.
                /// </summary>
                /// <returns><c>true</c> if moved to the next value.</returns>
                public bool MoveNext()
                {
                    if (_count == 0)
                        return false;

                    int i = _index + 1;
                    while (i < _count)
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

        private Entry* _buffer;
        private int _capacity;
        private int _count;

        private readonly int _allocatorID;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSortedMap{TKey, TValue}"/> struct.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        /// <exception cref="ArgumentException">If the capacity is negative or 0.</exception>
        public NativeSortedMap(int initialCapacity) : this(initialCapacity, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSortedMap{TKey, TValue}"/> struct.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        /// <param name="allocator">The allocator.</param>
        /// <exception cref="ArgumentException">If the capacity is negative or 0, or if the allocator is not in cache.</exception>
        public NativeSortedMap(int initialCapacity, Allocator allocator)
        {
            if (initialCapacity <= 0)
            {
                throw new ArgumentException("initialCapacity should be greater than 0.", nameof(initialCapacity));
            }

            if (Allocator.IsCached(allocator) is false)
            {
                throw new ArgumentException("Allocator is not in cache.", nameof(allocator));
            }

            _buffer = (Entry*)allocator.Allocate(initialCapacity, sizeof(Entry));
            _capacity = initialCapacity;
            _count = 0;
            _allocatorID = allocator.ID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSortedMap{TKey, TValue}" /> struct.
        /// </summary>
        /// <param name="elements">The initial elements.</param>
        public NativeSortedMap(Span<(TKey, TValue)> elements) : this(elements, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeSortedMap{TKey, TValue}" /> struct.
        /// </summary>
        /// <param name="elements">The initial elements.</param>
        /// <param name="allocator">The allocator.</param>
        /// <exception cref="ArgumentException">If the capacity is negative or 0, or if the allocator is not in cache.</exception>
        public NativeSortedMap(Span<(TKey, TValue)> elements, Allocator allocator)
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
                _buffer = (Entry*)allocator.Allocate(elements.Length, sizeof(Entry));
                _capacity = elements.Length;
                _count = 0;
                _allocatorID = allocator.ID;

                foreach (var e in elements)
                {
                    Add(e.Item1, e.Item2);
                }
            }
        }

        internal NativeSortedMap(void* pointer, int length, Allocator allocator)
        {
            Debug.Assert(pointer != null);
            Debug.Assert(length > 0);
            Debug.Assert(Allocator.IsCached(allocator));

            _buffer = (Entry*)pointer;
            _count = length;
            _capacity = length;
            _allocatorID = allocator.ID;
        }

        private NativeSortedMap(ref NativeSortedMap<TKey, TValue> map)
        {
            Debug.Assert(map.IsValid);

            Allocator allocator = map.GetAllocator()!;
            Entry* buffer = allocator.Allocate<Entry>(map._capacity);
            Unsafe.CopyBlockUnaligned(buffer, map._buffer, (uint)(sizeof(Entry) * map._capacity));

            _buffer = buffer;
            _capacity = map._capacity;
            _count = map._count;
            _allocatorID = map._allocatorID;
        }

        /// <summary>
        /// Gets the number of elements in this map.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public readonly int Length => _count;

        /// <summary>
        /// Gets the number of elements this map can hold before resize.
        /// </summary>
        /// <value>
        /// The capacity.
        /// </value>
        public readonly int Capacity => _capacity;

        /// <summary>
        /// Gets a value indicating whether this map have elements.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this map is empty; otherwise, <c>false</c>.
        /// </value>
        public readonly bool IsEmpty => _count == 0;

        /// <summary>
        /// Checks if this map is allocated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this map is valid; otherwise, <c>false</c>.
        /// </value>
        public readonly bool IsValid => _buffer != null;

        /// <summary>
        /// Gets the allocator.
        /// </summary>
        /// <returns>The allocator used for this map.</returns>
        public Allocator? GetAllocator()
        {
            return Allocator.GetAllocatorByID(_allocatorID);
        }

        /// <summary>
        /// Gets the first key of this map.
        /// </summary>
        public readonly ref TKey FirstKey
        {
            get
            {
                if (_buffer == null)
                {
                    throw new InvalidOperationException("NativeSortedMap is invalid");
                }

                if (_count == 0)
                {
                    throw new InvalidOperationException("NativeSortedMap is empty");
                }

                return ref _buffer[0].key;
            }
        }

        /// <summary>
        /// Gets the last key of this map.
        /// </summary>
        public readonly ref TKey LastKey
        {
            get
            {
                if (_buffer == null)
                {
                    throw new InvalidOperationException("NativeSortedMap is invalid");
                }

                if (_count == 0)
                {
                    throw new InvalidOperationException("NativeSortedMap is empty");
                }

                return ref _buffer[_count - 1].key;
            }
        }

        /// <summary>
        /// Gets a collection of the keys of this map.
        /// </summary>
        public KeyCollection Keys => new KeyCollection(ref this);

        /// <summary>
        /// Gets a collection of the values of this map.
        /// </summary>
        public ValueCollection Values => new ValueCollection(ref this);

        /// <summary>
        /// Gets the value associated to the key.
        /// </summary>
        public TValue this[TKey key]
        {
            get
            {
                if (_buffer == null)
                {
                    throw new InvalidOperationException("NativeSortedMap is invalid");
                }

                int index = BinarySearch(key);
                if(index >= 0)
                {
                    return _buffer[index].value;
                }

                throw new KeyNotFoundException(key.ToString());
            }

            set
            {
                TryInsert(key, value, InsertMode.Any);
            }
        }

        /// <summary>
        /// Adds the specified key and value to the map.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">If the key is duplicated.</exception>
        public void Add(TKey key, TValue value)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            if (!TryInsert(key, value, InsertMode.Add))
            {
                throw new ArgumentException("Duplicated key", nameof(key));
            }
        }

        /// <summary>
        /// Attemps to add the specified key and value to map.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the key and value were added, otherwise <c>false</c>.</returns>
        public bool TryAdd(TKey key, TValue value)
        {
            return TryInsert(key, value, InsertMode.Add);
        }

        /// <summary>
        /// Adds the specified key and value to the map of replace the key value if already exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void AddOrUpdate(TKey key, TValue value)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            TryInsert(key, value, InsertMode.Any);
        }

        /// <summary>
        /// Replaces the value associated to the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value was replaced.</returns>
        public bool Replace(TKey key, TValue value)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            if (_count == 0)
            {
                return false;
            }

            int index = BinarySearch(key);

            if (index >= 0)
            {
                _buffer[index].value = value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the specified key and the value associated to it.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the key and value were removed.</returns>
        public bool Remove(TKey key)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            if (_count == 0)
            {
                return false;
            }

            int index = BinarySearch(key);

            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the 0-index key-value in the map.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            if (index < 0 || index > _count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index.ToString());
            }

            _count--;
            if (_count > index)
            {
                int length = _count - index;
                Entry* src = _buffer + index + 1;
                Entry* dst = _buffer + index;
                Unsafe.CopyBlock(dst, src, (uint)(sizeof(Entry) * length));
            }

            _buffer[_count] = default;
        }

        /// <summary>
        /// Attemps to get the value associated to the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the key was found, otherwise <c>false</c>.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = BinarySearch(key);
            if (index >= 0)
            {
                value = _buffer[index].value;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attemps to get a reference to the value associated to the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the key was found, otherwise <c>false</c>.</returns>
        public bool TryGetValueReference(TKey key, out ByReference<TValue> value)
        {
            int index = BinarySearch(key);
            if (index >= 0)
            {
                value = new ByReference<TValue>(ref _buffer[index].value);
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the value associated to the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value associated to the key.</returns>
        /// <exception cref="KeyNotFoundException">If the key don't exists in the map.</exception>
        public TValue GetValue(TKey key)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            if (!TryGetValue(key, out TValue value))
            {
                throw new KeyNotFoundException(key.ToString());
            }

            return value;
        }

        /// <summary>
        /// Gets the value associated to the specified key or the <c>defaultValue</c> if not found.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The found value or the default.</returns>
        public TValue GetValueOrDefault(TKey key, TValue defaultValue)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            if (TryGetValue(key, out TValue value))
            {
                return value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets a reference to the value associated to the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A reference to the value associated to the key.</returns>
        /// <exception cref="KeyNotFoundException">If the key don't exists in the map.</exception>
        public ref TValue GetValueReference(TKey key)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            int index = BinarySearch(key);

            if (index >= 0)
            {
                return ref _buffer[index].value;
            }

            throw new KeyNotFoundException(key.ToString());
        }

        /// <summary>
        /// Gets a lower key than the specified on the map or null if not found.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The key with a lower value than the specified or null if not found.</returns>
        public TKey? LowerKey(TKey key)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            int index = BinarySearch(key);
            if (index >= 0)
            {
                int i = index - 1;

                if(i < 0)
                {
                    return null;
                }

                return _buffer[i].key;
            }

            return _buffer[_count - 1].key;
        }

        /// <summary>
        /// Gets a higher key than the specified on the map or null if not found.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The key with a higher value than the specified or null if not found.</returns>
        public TKey? HigherKey(TKey key)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            int index = BinarySearch(key);
            if (index >= 0)
            {
                int i = index + 1;

                if(i >= _count)
                {
                    return null;
                }

                return _buffer[i].key;
            }

            return _buffer[0].key;
        }

        /// <summary>
        /// Gets a range from this map.
        /// </summary>
        /// <param name="fromKey">Start key of the range (inclusive).</param>
        /// <param name="toKey">End key of the range (inclusive).</param>
        /// <returns>A range of this map within the given values.</returns>
        public NativeSortedMap<TKey, TValue> SubMap(TKey fromKey, TKey toKey)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            var comparer = Comparer<TKey>.Default;
            int comp = comparer.Compare(fromKey, toKey);
            if (comp > 0)
            {
                throw new ArgumentException($"fromKey is greater than toKey: {fromKey} > {toKey}");
            }

            if(comp == 0)
            {
                return default;
            }

            int startIndex = BinarySearch(fromKey);
            int toIndex = BinarySearch(toKey);

            if (startIndex < 0)
            {
                startIndex = ~startIndex;
            }

            if (toIndex < 0)
            {
                toIndex = ~toIndex;
            }

            int length = toIndex - startIndex + 1;
            Entry* buffer = GetAllocator()!.Allocate<Entry>(length);
            Unsafe.CopyBlockUnaligned(buffer, _buffer + startIndex, (uint)(sizeof(Entry) * length));
            return new NativeSortedMap<TKey, TValue>(buffer, length, GetAllocator()!);
        }

        /// <summary>
        /// Gets a sub map within the specified 0-index range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns>A sub map within the given range.</returns>
        public NativeSortedMap<TKey, TValue> SubMap(Range range)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            var (startIndex, length) = range.GetOffsetAndLength(_count);

            if (length == 0)
            {
                return default;
            }

            Entry* buffer = GetAllocator()!.Allocate<Entry>(length);
            Unsafe.CopyBlockUnaligned(buffer, _buffer + startIndex, (uint)(sizeof(Entry) * length));
            return new NativeSortedMap<TKey, TValue>(buffer, length, GetAllocator()!);
        }

        /// <summary>
        /// Clears the content of this map.
        /// </summary>
        public void Clear()
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            if (_count == 0)
            {
                return;
            }

            _count = 0;
        }

        /// <summary>
        /// Gets the key-value at the specified 0-index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The key-value at the specified index.</returns>
        public ref KeyValuePair<TKey, TValue> ElementAt(int index)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            if (index < 0 || index > _count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index.ToString());
            }

            return ref Unsafe.As<Entry, KeyValuePair<TKey, TValue>>(ref _buffer[index]);
        }

        /// <summary>
        /// Determines whether the map contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>
        ///   <c>true</c> if the map contains the specified key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            if (_count == 0)
            {
                return false;
            }

            return BinarySearch(key) >= 0;
        }

        /// <summary>
        /// Determines whether the map contains the specified value.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <returns>
        ///   <c>true</c> if the map contains the specified value; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsValue(TValue value)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            if (_count == 0)
            {
                return false;
            }

            var comparer = EqualityComparer<TValue>.Default;

            for (int i = 0; i < _count; i++)
            {
                if (comparer.Equals(_buffer[i].value, value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the 0-index of the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>The index of the key or -1 if not found.</returns>
        public int IndexOfKey(TKey key)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            int index = BinarySearch(key);
            return index >= 0 ? index : -1;
        }

        /// <summary>
        /// Gets the 0-index of the specified value.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <returns>The index of the value or -1 if not found.</returns>
        public int IndexOfValue(TValue value)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            var comparer = EqualityComparer<TValue>.Default;

            for(int i = 0; i < _count; i++)
            {
                if(comparer.Equals(_buffer[i].value, value))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Removes the excess space in this map.
        /// </summary>
        public void TrimExcess()
        {
            TrimExcess(_count);
        }

        /// <summary>
        /// Removes the excess space in this map.
        /// </summary>
        /// <param name="capacity">The min capacity.</param>
        public void TrimExcess(int capacity)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            if (capacity < _count)
            {
                return;
            }

            Resize(capacity);
        }

        /// <summary>
        /// Ensures this map can hold the specified amount of elements before resize.
        /// </summary>
        /// <param name="capacity">The min capacity.</param>
        public void EnsureCapacity(int capacity)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            if (capacity > _capacity)
            {
                Resize(capacity);
            }
        }

        /// <summary>
        /// Copies the content of this map to a <see cref="Span{T}" />.
        /// </summary>
        /// <param name="span">The destination span to copy the data.</param>
        /// <param name="destinationIndex">Start index of the destination where start to copy.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentException">Span is empty</exception>
        /// <exception cref="InvalidOperationException">NativeSortedMap is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the range provided by the destination index and count is invalid.</exception>
        public void CopyTo(in Span<KeyValuePair<TKey, TValue>> span, int destinationIndex, int count)
        {
            if (span.IsEmpty)
                throw new ArgumentException("Span is empty");

            if (_buffer == null)
                throw new InvalidOperationException("NativeSortedMap is invalid");

            if (destinationIndex < 0 || destinationIndex > span.Length)
                throw new ArgumentOutOfRangeException(nameof(destinationIndex), destinationIndex.ToString());

            if (count < 0 || count > _count || count > (span.Length - destinationIndex))
                throw new ArgumentOutOfRangeException(nameof(count), count.ToString());

            for(int i = 0; count > 0; --count, ++i)
            {
                ref Entry entry = ref _buffer[i];
                span[destinationIndex++] = new KeyValuePair<TKey, TValue>(entry.key, entry.value);
            }
        }

        /// <summary>
        /// Allocates an array with the elements of this map.
        /// </summary>
        /// <returns>An newly allocated array with the elements of this instance.</returns>
        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            if (_count == 0)
            {
                return Array.Empty<KeyValuePair<TKey, TValue>>();
            }

            KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[_count];
            for (int i = 0; i < _count; i++)
            {
                ref Entry entry = ref _buffer[i];
                array[i] = new KeyValuePair<TKey, TValue>(entry.key, entry.value);
            }
            return array;
        }

        /// <summary>
        /// Creates a new <see cref="NativeArray{T}"/> with the elements of this map.
        /// </summary>
        /// <returns>A new array with the elements of this instance.</returns>
        public NativeArray<KeyValuePair<TKey, TValue>> ToNativeArray()
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            if (_count == 0)
            {
                return default;
            }

            NativeArray<KeyValuePair<TKey, TValue>> array = new NativeArray<KeyValuePair<TKey, TValue>>(_count, GetAllocator()!);
            int i = 0;
            foreach (ref var e in this)
            {
                array[i] = e;
                ++i;
            }
            return array;
        }

        /// <summary>
        /// Creates a new <see cref="NativeArray{T}"/> with the elements of this set and dispose this map.
        /// </summary>
        /// <param name="createNewArrayIfNeeded">If <c>true</c> a new array will be created if the capacity of this
        /// map is different than its length; otherwise is guaranteed the new array will use this stack memory.</param>
        /// <returns>A newly created array with this list elements.</returns>
        public NativeArray<KeyValuePair<TKey, TValue>> ToNativeArrayAndDispose(bool createNewArrayIfNeeded = true)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeSortedMap is invalid");
            }

            if (_count == _capacity || !createNewArrayIfNeeded)
            {
                // NativeArray will owns this instance memory
                NativeArray<KeyValuePair<TKey, TValue>> array = new NativeArray<KeyValuePair<TKey, TValue>>(_buffer, _capacity, GetAllocator()!);

                // Not actual dispose, just invalidate this instance
                this = default;
                return array;
            }
            else
            {
                NativeArray<KeyValuePair<TKey, TValue>> array = ToNativeArray();
                Dispose();
                return array;
            }
        }

        private void Resize()
        {
            Resize(_capacity * 2);
        }

        private void Resize(int newCapacity)
        {
            Debug.Assert(_buffer != null);
            _buffer = GetAllocator()!.Reallocate<Entry>(_buffer, newCapacity);
            _capacity = newCapacity;
        }

        private bool TryInsert(TKey key, TValue value, InsertMode mode)
        {
            if(_buffer == null)
            {
                return false;
            }

            if (_count == _capacity)
            {
                Resize();
            }

            if (_count == 0 && ((mode == InsertMode.Add || mode == InsertMode.Any)))
            {
                _buffer[_count++] = new Entry(key, value);
                return true;
            }

            var comparer = EqualityComparer<TKey>.Default;
            int index = BinarySearch(key);

            if(mode == InsertMode.Replace || mode == InsertMode.Any)
            {
                if(index < 0)
                {
                    index = ~index;
                }

                if(comparer.Equals(_buffer[index].key, key))
                {
                    _buffer[index].value = value;
                    return true;
                }

                if(mode == InsertMode.Replace)
                {
                    return false;
                }
            }

            if (index < 0)
            {
                index = ~index;
            }

            if (comparer.Equals(_buffer[index].key, key))
            {
                return false;
            }

            int length = _capacity - index;
            Entry* src = _buffer + index;
            Entry* dst = src + 1;
            Unsafe.CopyBlock(dst, src, (uint)(sizeof(Entry) * length));
            _buffer[index] = new Entry(key, value);
            _count++;

            return true;
        }

        private readonly int BinarySearch(TKey key)
        {
            int start = 0;
            int end = _count - 1;

            var comparer = Comparer<TKey>.Default;

            while(start <= end)
            {
                int mid = start + ((end - start) >> 1);
                int comp = comparer.Compare(_buffer[mid].key, key);

                if(comp == 0)
                {
                    return mid;
                }
                if(comp < 0)
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

        /// <summary>
        /// Gets a string representation of the elements of this map.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this map.
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
                ref Entry entry = ref _buffer[i];
                sb.Append('{');
                sb.Append(entry.key.ToString());
                sb.Append(", ");
                sb.Append(entry.value.ToString());
                sb.Append('}');

                if (i != _count - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(']');
            return StringBuilderCache.ToStringAndRelease(ref sb!);
        }
        
        /// <summary>
        /// Releases the resouces used for this map.
        /// </summary>
        public void Dispose()
        {
            if (_buffer == null)
            {
                return;
            }

            if (Allocator.IsCached(_allocatorID))
            {
                GetAllocator()!.Free(_buffer);
                this = default;
            }
        }

        /// <summary>
        /// Gets an enumerator over the elements of this map.
        /// </summary>
        /// <returns>An enumerator over the elements of the map.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator()
        {
            return _buffer == null ? default : new Enumerator(ref this);
        }

        /// <summary>
        /// An enumerator over the elements of a <see cref="NativeSortedMap{TKey, TValue}"/>.
        /// </summary>
        public ref struct Enumerator
        {
            private Entry* _entries;
            private int _length;
            private int _index;

            /// <summary>
            /// Initializes a new instance of the <see cref="NativeSortedMap{TKey, TValue}.Enumerator" /> struct.
            /// </summary>
            /// <param name="map">The map.</param>
            internal Enumerator(ref NativeSortedMap<TKey, TValue> map)
            {
                _entries = map._buffer;
                _length = map._count;
                _index = -1;
            }

            /// <summary>
            /// Gets the current value.
            /// </summary>
            public ref KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    if (_index < 0 || _index > _length)
                        throw new ArgumentOutOfRangeException("index", _index.ToString());

                    return ref Unsafe.As<Entry, KeyValuePair<TKey, TValue>>(ref _entries[_index]);
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
            /// <returns><c>true</c> if moved to the next value.</returns>
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
    }
}
