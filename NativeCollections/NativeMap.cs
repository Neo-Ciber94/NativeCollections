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

    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeMapDebugView<,>))]
    public unsafe struct NativeMap<TKey, TValue> : IDisposable where TKey : unmanaged where TValue : unmanaged
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

        public ref struct KeyCollection
        {
            private NativeMap<TKey, TValue> _map;

            internal KeyCollection(ref NativeMap<TKey, TValue> map)
            {
                _map = map;
            }

            public readonly int Length => _map.Length;

            public readonly bool Contains(TKey key) => _map.ContainsKey(key);

            public readonly void CopyTo(in Span<TKey> span)
            {
                CopyTo(span, 0, Length);
            }

            public readonly void CopyTo(in Span<TKey> span, int count)
            {
                CopyTo(span, 0, count);
            }

            public readonly void CopyTo(in Span<TKey> span, int startIndex, int count)
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
                        span[startIndex++] = entry.key;
                    }

                    i++;
                }
                while (i < count);
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(ref _map);
            }

            public ref struct Enumerator
            {
                private Entry* _entries;
                private int _count;
                private int _index;

                public Enumerator(ref NativeMap<TKey, TValue> map)
                {
                    _entries = map._buffer;
                    _count = map._count;
                    _index = -1;
                }

                public readonly ref TValue Current
                {
                    get
                    {
                        if (_index < 0 || _index > _count)
                            throw new ArgumentOutOfRangeException("index", _index.ToString());

                        return ref _entries[_index].value;
                    }
                }

                public void Dispose()
                {
                    if (_entries == null)
                        return;

                    _entries = null;
                    _count = 0;
                    _index = -1;
                }

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

                public void Reset()
                {
                    _index = -1;
                }
            }
        }

        public ref struct ValueCollection
        {
            private NativeMap<TKey, TValue> _map;

            internal ValueCollection(ref NativeMap<TKey, TValue> map)
            {
                _map = map;
            }

            public readonly int Length => _map.Length;

            public readonly bool Contains(TValue value) => _map.ContainsValue(value);

            public readonly void CopyTo(in Span<TValue> span)
            {
                CopyTo(span, 0, Length);
            }

            public readonly void CopyTo(in Span<TValue> span, int count)
            {
                CopyTo(span, 0, count);
            }

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

            public Enumerator GetEnumerator()
            {
                return new Enumerator(ref _map);
            }

            public ref struct Enumerator
            {
                private Entry* _entries;
                private int _count;
                private int _index;

                public Enumerator(ref NativeMap<TKey, TValue> map)
                {
                    _entries = map._buffer;
                    _count = map._count;
                    _index = -1;
                }

                public readonly ref TKey Current
                {
                    get
                    {
                        if (_index < 0 || _index > _count)
                            throw new ArgumentOutOfRangeException("index", _index.ToString());

                        return ref _entries[_index].key;
                    }
                }

                public void Dispose()
                {
                    if (_entries == null)
                        return;

                    _entries = null;
                    _count = 0;
                    _index = -1;
                }

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

                public void Reset()
                {
                    _index = -1;
                }
            }
        }

        public ref struct Enumerator
        {
            private Entry* _entries;
            private int _count;
            private int _index;

            public Enumerator(ref NativeMap<TKey, TValue> map)
            {
                _entries = map._buffer;
                _count = map._count;
                _index = -1;
            }

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

            public void Dispose()
            {
                if (_entries == null)
                    return;

                _entries = null;
                _count = 0;
                _index = -1;
            }

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

            public void Reset()
            {
                _index = -1;
            }
        }

        internal Entry* _buffer;
        private int _count;
        private int _capacity;
        private int _freeList;
        private int _freeCount;
        private int _allocatorID;

        public NativeMap(int initialCapacity) : this(initialCapacity, Allocator.Default) { }

        public NativeMap(int initialCapacity, Allocator allocator)
        {
            if (initialCapacity <= 0)
                throw new ArgumentException($"initialCapacity should be greater than 0: {initialCapacity}");

            _buffer = (Entry*)allocator.Allocate(initialCapacity, sizeof(Entry));
            _capacity = initialCapacity;
            _count = 0;
            _freeList = -1;
            _freeCount = 0;
            _allocatorID = allocator.ID;

            Initializate();
        }

        public NativeMap(Span<(TKey, TValue)> elements) : this(elements, Allocator.Default) { }

        public NativeMap(Span<(TKey, TValue)> elements, Allocator allocator)
        {
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

        public readonly int Length => _count - _freeCount;

        public readonly int Capacity => _capacity;

        public readonly bool IsValid => _buffer != null;

        public readonly bool IsEmpty => _count == 0;

        public Allocator? GetAllocator()
        {
            return Allocator.GetAllocatorByID(_allocatorID);
        }

        public KeyCollection Keys => new KeyCollection(ref this);

        public ValueCollection Values => new ValueCollection(ref this);

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

        public void Add(TKey key, TValue value)
        {
            if (!TryInsert(key, value, InsertMode.Add))
                throw new InvalidOperationException($"Duplicated key: {key}");
        }

        public bool TryAdd(TKey key, TValue value)
        {
            return TryInsert(key, value, InsertMode.Add);
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            TryInsert(key, value, InsertMode.Any);
        }

        public bool Replace(TKey key, TValue newValue)
        {
            if (TryInsert(key, newValue, InsertMode.Replace))
                return true;

            return false;
        }

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
        
        public TValue GetValue(TKey key)
        {
            if (!TryGetValue(key, out TValue value))
            {
                throw new KeyNotFoundException(key.ToString());
            }

            return value;
        }

        public TValue GetValueOrDefault(TKey key, TValue defaultValue)
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }

            return defaultValue;
        }

        public readonly bool ContainsKey(in TKey key)
        {
            return FindEntry(key) >= 0;
        }

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

        public readonly void CopyTo(in Span<KeyValuePair<TKey, TValue>> span)
        {
            CopyTo(span, 0, Length);
        }

        public readonly void CopyTo(in Span<KeyValuePair<TKey, TValue>> span, int count)
        {
            CopyTo(span, 0, count);
        }

        public readonly void CopyTo(in Span<KeyValuePair<TKey, TValue>> span, int startIndex, int count)
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
                ref Entry entry = ref _buffer[j++];
                if (entry.hashCode >= 0)
                {
                    span[startIndex++] = new KeyValuePair<TKey, TValue>(entry.key, entry.value);
                }

                i++;
            }
            while (i < count);
        }

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

        public void TrimExcess()
        {
            TrimExcess(Length);
        }

        public void TrimExcess(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException(nameof(capacity));

            if (capacity <= Length)
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

        public void EnsureCapacity(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException(nameof(capacity));

            if (capacity > _capacity)
            {
                Resize(capacity);
            }
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

        public void Dispose()
        {
            if (_buffer == null)
                return;

            if (Allocator.IsCached(_allocatorID))
            {
                GetAllocator()!.Free(_buffer);
                _buffer = null;
                _capacity = 0;
                _count = 0;
                _freeCount = 0;
                _freeList = 0;
            }
        }

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

        public Enumerator GetEnumerator()
        {
            return new Enumerator(ref this);
        }
    }
}
