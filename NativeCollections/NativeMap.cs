using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using NativeCollections.Allocators;
using NativeCollections.Utility;

namespace NativeCollections
{
    internal enum InsertMode : byte
    {
        Any,
        Add,
        Replace
    }

    /// <summary>
    /// Represents a collection of key-values where each key is associated with a single value.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="NativeCollections.INativeContainer{System.Collections.Generic.KeyValuePair{TKey, TValue}}" />
    /// <seealso cref="System.IDisposable" />
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeMapDebugView<,>))]
    public unsafe struct NativeMap<TKey, TValue> : INativeContainer<KeyValuePair<TKey, TValue>>, IDisposable where TKey : unmanaged where TValue : unmanaged
    {
        internal struct Entry
        {
            public TKey key;
            public TValue value;
            public int hashCode;
            public int next;
            public int bucket;

            public override string ToString()
            {
                return $"[{key}, {value}]";
            }
        }

        /// <summary>
        /// A collection of the keys of a <see cref="NativeMap{TKey, TValue}"/>.
        /// </summary>
        public ref struct KeyCollection
        {
            private NativeMap<TKey, TValue> _map;

            internal KeyCollection(ref NativeMap<TKey, TValue> map)
            {
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

                int i = 0;
                int j = 0;

                do
                {
                    ref Entry entry = ref _map._buffer[j++];
                    if (entry.hashCode >= 0)
                    {
                        span[startIndex++] = entry.key;
                    }

                    i++;
                }
                while (i < count);
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

                internal Enumerator(ref NativeMap<TKey, TValue> map)
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

        /// <summary>
        /// A collection of the values of a <see cref="NativeMap{TKey, TValue}"/>.
        /// </summary>
        public ref struct ValueCollection
        {
            private NativeMap<TKey, TValue> _map;

            internal ValueCollection(ref NativeMap<TKey, TValue> map)
            {
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

                int i = 0;
                int j = 0;

                do
                {
                    ref Entry entry = ref _map._buffer[j++];
                    if (entry.hashCode >= 0)
                    {
                        span[startIndex++] = entry.value;
                    }

                    i++;
                }
                while (i < count);
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

                internal Enumerator(ref NativeMap<TKey, TValue> map)
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

        internal Entry* _buffer;
        private int _capacity;
        private int _count;
        private int _freeList;
        private int _freeCount;

        private readonly int _allocatorID;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeMap{TKey, TValue}" /> struct.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        public NativeMap(int initialCapacity) : this(initialCapacity, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeMap{TKey, TValue}" /> struct.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        /// <param name="allocator">The allocator.</param>
        public NativeMap(int initialCapacity, Allocator allocator)
        {
            if (initialCapacity <= 0)
            {
                throw new ArgumentException($"initialCapacity should be greater than 0: {initialCapacity}");
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
        /// Initializes a new instance of the <see cref="NativeMap{TKey, TValue}" /> struct.
        /// </summary>
        /// <param name="elements">The initial elements.</param>
        public NativeMap(Span<(TKey, TValue)> elements) : this(elements, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeMap{TKey, TValue}" /> struct.
        /// </summary>
        /// <param name="elements">The initial elements.</param>
        /// <param name="allocator">The allocator.</param>
        public NativeMap(Span<(TKey, TValue)> elements, Allocator allocator)
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
                    Add(e.Item1, e.Item2);
                }
            }
        }

        /// <summary>
        /// Gets the number of elements on this map.
        /// </summary>
        public readonly int Length => _count - _freeCount;

        /// <summary>
        /// Gets the number of elements this map can hold without resize.
        /// </summary>
        public readonly int Capacity => _capacity;

        /// <summary>
        /// Checks if this map is allocated.
        /// </summary>
        public readonly bool IsValid => _buffer != null;

        /// <summary>
        /// Determines if this map have elements.
        /// </summary>
        public readonly bool IsEmpty => _count == 0;

        /// <summary>
        /// Gets the allocator.
        /// </summary>
        /// <returns>The allocator used for this map.</returns>
        public Allocator? GetAllocator()
        {
            return Allocator.GetAllocatorByID(_allocatorID);
        }

        public KeyCollection Keys => new KeyCollection(ref this);

        public ValueCollection Values => new ValueCollection(ref this);

        /// <summary>
        /// Gets or sets the <see cref="TValue" /> associated to the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="TValue" />.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The value associated to the key.</returns>
        /// <exception cref="KeyNotFoundException"> if the key don't exists when the getter is used.</exception>
        public TValue this[TKey key]
        {
            readonly get
            {
                var index = FindEntry(key);
                if (index >= 0)
                {
                    return _buffer[index].value;
                }

                throw new KeyNotFoundException(key.ToString());
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => AddOrUpdate(key, value);
        }

        /// <summary>
        /// Adds the specified key and value to the map.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="InvalidOperationException">If the key already exists.</exception>
        public void Add(TKey key, TValue value)
        {
            if (!TryInsert(key, value, InsertMode.Add))
                throw new InvalidOperationException($"Duplicated key: {key}");
        }

        /// <summary>
        /// Attemps to add the specified key and value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the pair was added or <c>false</c> if the key already exists.</returns>
        public bool TryAdd(TKey key, TValue value)
        {
            return TryInsert(key, value, InsertMode.Add);
        }

        /// <summary>
        /// Adds the specified key and value pair to the map, or update the key associated value if already exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void AddOrUpdate(TKey key, TValue value)
        {
            TryInsert(key, value, InsertMode.Any);
        }

        /// <summary>
        /// Replaces the value associated to the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns><c>true</c> if the value was replaced otherwise <c>false</c>.</returns>
        public bool Replace(TKey key, TValue newValue)
        {
            if (TryInsert(key, newValue, InsertMode.Replace))
                return true;

            return false;
        }

        /// <summary>
        /// Removes key and the value associated to it.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the key and value were removed.</returns>
        public bool Remove(TKey key)
        {
            var comparer = EqualityComparer<TKey>.Default;
            var hashCode = GetHash(key);
            var bucket = GetBucket(hashCode, _capacity);
            var index = _buffer[bucket].bucket;
            int last = -1;

            while (index >= 0)
            {
                ref Entry entry = ref _buffer[index];

                if (comparer.Equals(entry.key, key) && entry.hashCode == hashCode)
                {
                    if (last >= 0)
                    {
                        _buffer[last].next = _buffer[index].next;
                    }
                    else
                    {
                        _buffer[bucket].bucket = _buffer[bucket].next;
                    }

                    entry.next = _freeList;
                    entry.hashCode = -1;

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
        /// Clears the content of this map.
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
        /// Attemps to get the value associated to the specifeid key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The ouput value.</param>
        /// <returns><c>true</c> if the key exists thus its value will be returned.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = FindEntry(key);
            if (index >= 0)
            {
                value = _buffer[index].value;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attemps to get a reference to the value associated to the specifeid key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The ouput value reference.</param>
        /// <returns><c>true</c> if the key exists thus its value will be returned.</returns>
        public bool TryGetValueReference(TKey key, out ByReference<TValue> value)
        {
            int index = FindEntry(key);
            if (index >= 0)
            {
                value = new ByReference<TValue>(ref _buffer[index].value);
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the value associated to the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value associated to the key.</returns>
        /// <exception cref="KeyNotFoundException"> if the key don't exists.</exception>
        public TValue GetValue(TKey key)
        {
            if (!TryGetValue(key, out TValue value))
            {
                throw new KeyNotFoundException(key.ToString());
            }

            return value;
        }

        /// <summary>
        /// Gets the value associated to the key or default given value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value associated to the key or the default value if the key don't exists.</returns>
        public TValue GetValueOrDefault(TKey key, TValue defaultValue)
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets a reference to the value associated to the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A reference to the value associated to the key.</returns>
        /// <exception cref="KeyNotFoundException"> if the key don't exists.</exception>
        public ref TValue GetValueReference(TKey key)
        {
            int index = FindEntry(key);

            if (index >= 0)
            {
                return ref _buffer[index].value;
            }

            throw new KeyNotFoundException(key.ToString());
        }

        /// <summary>
        /// Determines whether the map contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the map contains the specified key; otherwise, <c>false</c>.
        /// </returns>
        public readonly bool ContainsKey(in TKey key)
        {
            return FindEntry(key) >= 0;
        }

        /// <summary>
        /// Determines whether the map contains the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the map contains the specified value; otherwise, <c>false</c>.
        /// </returns>
        public readonly bool ContainsValue(in TValue value)
        {
            var comparer = EqualityComparer<TValue>.Default;

            for (int i = 0; i < _count; i++)
            {
                ref Entry entry = ref _buffer[i];
                if (entry.hashCode >= 0 && comparer.Equals(entry.value, value))
                    return true;

            }

            return false;
        }

        /// <summary>
        /// Copies the <see cref="KeyValuePair{TKey, TValue}"/> this map contains to the specified <see cref="Span{T}"/>.
        /// </summary>
        /// <param name="span">The destination span.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <exception cref="ArgumentException">If the span is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the range startIndex and count are out of range.</exception>
        public readonly void CopyTo(in Span<KeyValuePair<TKey, TValue>> span, int startIndex, int count)
        {
            if (span.IsEmpty)
                throw new ArgumentException("Span is empty", nameof(span));

            if (startIndex < 0 || startIndex > span.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex.ToString());

            if (count < 0 || count > Length)
                throw new ArgumentOutOfRangeException(nameof(count), count.ToString());

            int i = 0;
            int j = 0;

            do
            {
                ref Entry entry = ref _buffer[j++];
                if (entry.hashCode >= 0)
                {
                    span[startIndex++] = new KeyValuePair<TKey, TValue>(entry.key, entry.value);
                }

                i++;
            }
            while (i < count);
        }

        /// <summary>
        /// Allocates a new array with the key-values of this map.
        /// </summary>
        /// <returns>An array with the key-values of this map.</returns>
        public readonly KeyValuePair<TKey, TValue>[] ToArray()
        {
            if (_count == 0)
            {
                return Array.Empty<KeyValuePair<TKey, TValue>>();
            }

            KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[Length];

            int j = 0;
            for (int i = 0; i < _count; i++)
            {
                ref Entry entry = ref _buffer[i];
                if (entry.hashCode >= 0)
                {
                    array[j++] = new KeyValuePair<TKey, TValue>(entry.key, entry.value);
                }
            }
            return array;
        }

        /// <summary>
        /// Removes the excess space from this map.
        /// </summary>
        public void TrimExcess()
        {
            TrimExcess(Length);
        }

        /// <summary>
        /// Removes the excess space from this map.
        /// </summary>
        /// <param name="capacity">The min capacity.</param>
        public void TrimExcess(int capacity)
        {
            if (capacity < Length)
            {
                return;
            }

            Entry* newBuffer = GetAllocator()!.Allocate<Entry>(capacity);
            Unsafe.CopyBlock(newBuffer, _buffer, (uint)(Unsafe.SizeOf<Entry>() * _count));

            // Free old buffer
            GetAllocator()!.Free(_buffer);

            for (int i = 0; i < capacity; i++)
            {
                newBuffer[i].bucket = -1;
            }

            int index = 0;
            int count = Length;

            for (int i = 0; i < count; i++)
            {
                ref Entry entry = ref newBuffer[i];
                int hashCode = GetHash(entry.key);

                if (hashCode >= 0)
                {
                    int bucket = GetBucket(hashCode, capacity);
                    newBuffer[index] = entry;
                    newBuffer[index].next = bucket;
                    newBuffer[bucket].bucket = index;
                    index++;
                }
            }

            _freeCount = 0;
            _freeList = -1;
            _count = count;
            _capacity = capacity;
        }

        /// <summary>
        /// Ensures this map can hold the specified amount of elements without resize.
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
        /// Releases all the resouces used for this map.
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
        /// Gets a string representation of the key-values of this map.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (_buffer == null)
            {
                return "[Invalid]";
            }

            if (_count == 0)
            {
                return "[]";
            }

            // TODO: Reduce StringBuilderCache calls overhead by implementing ToString() for (1..10) elements?

            StringBuilder sb = StringBuilderCache.Acquire();
            sb.Append('[');

            Enumerator enumerator = GetEnumerator();

            if (enumerator.MoveNext())
            {
                while (true)
                {
                    ref KeyValuePair<TKey, TValue> pair = ref enumerator.Current;
                    sb.Append('{');
                    sb.Append(pair.Key.ToString());
                    sb.Append(", ");
                    sb.Append(pair.Value.ToString());
                    sb.Append('}');

                    if (!enumerator.MoveNext())
                    {
                        break;
                    }

                    sb.Append(", ");
                }
            }

            sb.Append(']');
            return StringBuilderCache.ToStringAndRelease(ref sb!);
        }

        /// <summary>
        /// Gets an enumerator over the key-values of this map.
        /// </summary>
        /// <returns>An enumerator over this map key-values.</returns>
        public Enumerator GetEnumerator()
        {
            Debug.Assert(_buffer != null);

            if (_buffer == null)
            {
                return default;
            }

            return new Enumerator(ref this);
        }

        private bool TryInsert(TKey key, TValue value, InsertMode mode)
        {
            var comparer = EqualityComparer<TKey>.Default;
            var hashCode = GetHash(key);
            var bucket = GetBucket(hashCode, _capacity);
            var index = _buffer[bucket].bucket;

            while (index >= 0)
            {
                ref Entry entry = ref _buffer[index];
                switch (mode)
                {
                    case InsertMode.Any:
                    case InsertMode.Replace:
                        if (comparer.Equals(entry.key, key) && hashCode == entry.hashCode)
                        {
                            entry.value = value;
                            return true;
                        }
                        break;
                    case InsertMode.Add:
                        if (comparer.Equals(entry.key, key) && hashCode == entry.hashCode)
                            return false;
                        break;
                }

                index = entry.next;
            }

            if (mode == InsertMode.Replace)
            {
                return false;
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

            _buffer[index].key = key;
            _buffer[index].value = value;
            _buffer[index].hashCode = hashCode;
            _buffer[index].next = _buffer[bucket].bucket;
            _buffer[bucket].bucket = index;
            return true;
        }

        private readonly int FindEntry(in TKey key)
        {
            var comparer = EqualityComparer<TKey>.Default;
            var hashCode = GetHash(key);
            var bucket = GetBucket(hashCode, _capacity);
            int index = _buffer[bucket].bucket;

            while (index >= 0)
            {
                ref Entry entry = ref _buffer[index];
                if (comparer.Equals(entry.key, key) && entry.hashCode == hashCode)
                {
                    return index;
                }

                index = entry.next;
            }

            return -1;
        }

        private void Initializate()
        {
            for (int i = 0; i < _capacity; i++)
            {
                _buffer[i].bucket = -1;
            }
        }

        private void Resize()
        {
            Resize(_capacity * 2);
        }

        private void Resize(int newCapacity)
        {
            if (_buffer == null)
                return;

            Entry* newBuffer = GetAllocator()!.Allocate<Entry>(newCapacity);
            Unsafe.CopyBlock(newBuffer, _buffer, (uint)(sizeof(Entry) * _count));
            GetAllocator()!.Free(_buffer);

            for (int i = 0; i < newCapacity; i++)
            {
                newBuffer[i].bucket = -1;
            }

            for (int i = 0; i < _count; i++)
            {
                if (newBuffer[i].hashCode >= 0)
                {
                    int hashCode = GetHash(newBuffer[i].key);
                    int bucket = GetBucket(hashCode, newCapacity);
                    newBuffer[i].next = newBuffer[bucket].bucket;
                    newBuffer[bucket].bucket = i;
                }
            }

            _capacity = newCapacity;
            _buffer = newBuffer;
        }

        private readonly int GetHash(in TKey key)
        {
            return key.GetHashCode() & int.MaxValue;
        }

        private static int GetBucket(int hashCode, int capacity)
        {
            return hashCode % capacity;
        }

        /// <summary>
        /// An enumerator over the key-values of the map.
        /// </summary>
        public ref struct Enumerator
        {
            private Entry* _entries;
            private int _count;
            private int _index;

            /// <summary>
            /// Initializes a new instance of the <see cref="NativeMap{TKey, TValue}.Enumerator" /> struct.
            /// </summary>
            /// <param name="map">The map.</param>
            public Enumerator(ref NativeMap<TKey, TValue> map)
            {
                _entries = map._buffer;
                _count = map._count;
                _index = -1;
            }

            /// <summary>
            /// Gets a reference to the current key-value.
            /// </summary>
            public ref KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    if (_index < 0 || _index > _count)
                        throw new ArgumentOutOfRangeException("index", _index.ToString());

                    // KeyValuePair 'key' and 'value' are aligned with NativeMap.Entry 'key' and 'value'
                    return ref Unsafe.As<Entry, KeyValuePair<TKey, TValue>>(ref _entries[_index]);
                }
            }

            /// <summary>
            /// Invalidaes this enumerator.
            /// </summary>
            public void Dispose()
            {
                if (_entries == null)
                    return;

                _entries = null;
                _count = 0;
                _index = -1;
            }

            /// <summary>
            /// Moves to the next key-value.
            /// </summary>
            /// <returns></returns>
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
