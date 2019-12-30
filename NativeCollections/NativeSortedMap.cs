using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using NativeCollections.Allocators;
using NativeCollections.Utility;

namespace NativeCollections
{
    unsafe public struct NativeSortedMap<TKey, TValue> : IDisposable where TKey : unmanaged where TValue : unmanaged
    {
        internal struct Pair
        {
            public TKey key;
            public TValue value;

            public Pair(TKey key, TValue value)
            {
                this.key = key;
                this.value = value;
            }
        }

        public ref struct Enumerator
        {
            private Pair* _entries;
            private int _length;
            private int _index;

            public Enumerator(ref NativeSortedMap<TKey, TValue> map)
            {
                _entries = map._buffer;
                _length = map._count;
                _index = -1;
            }

            public ref KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    if (_index < 0 || _index > _length)
                        throw new ArgumentOutOfRangeException("index", _index.ToString());

                    return ref Unsafe.As<Pair, KeyValuePair<TKey, TValue>>(ref _entries[_index]);
                }
            }

            public void Dispose()
            {
                if (_entries == null)
                    return;

                _entries = null;
                _length = 0;
                _index = 0;
            }

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

            public void Reset()
            {
                _index = -1;
            }
        }

        private Pair* _buffer;
        private int _capacity;
        private int _count;
        private int _allocatorID;

        public NativeSortedMap(int initialCapacity) : this(initialCapacity, Allocator.Default) { }

        public NativeSortedMap(int initialCapacity, Allocator allocator)
        {
            if (initialCapacity <= 0)
                throw new ArgumentException("initialCapacity should be greater than 0.", nameof(initialCapacity));

            _buffer = (Pair*)allocator.Allocate(initialCapacity, sizeof(Pair));
            _capacity = initialCapacity;
            _count = 0;
            _allocatorID = allocator.ID;
        }

        public readonly int Length => _count;

        public readonly int Capacity => _capacity;

        public readonly bool IsEmpty => _count == 0;

        public readonly bool IsValid => _buffer != null;

        public Allocator? GetAllocator()
        {
            return Allocator.GetAllocatorByID(_allocatorID);
        }

        public readonly ref TValue Min
        {
            get
            {
                if (_count == 0)
                    throw new InvalidOperationException("NativeSortedMap is empty");

                return ref _buffer[0].value;
            }
        }

        public readonly ref TValue Max
        {
            get
            {
                if (_count == 0)
                    throw new InvalidOperationException("NativeSortedMap is empty");

                return ref _buffer[_count - 1].value;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
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

        public readonly ref KeyValuePair<TKey, TValue> this[int index]
        {
            get
            {
                if(index < 0 || index > _count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index.ToString());
                }

                return ref Unsafe.As<Pair, KeyValuePair<TKey, TValue>>(ref _buffer[index]);
            }
        }

        public void Add(TKey key, TValue value)
        {
            if(!TryInsert(key, value, InsertMode.Add))
            {
                throw new ArgumentException("Duplicated key", nameof(key));
            }
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            TryInsert(key, value, InsertMode.Any);
        }

        public bool Replace(TKey key, TValue value)
        {
            if(_count == 0)
            {
                return false;
            }

            int index = BinarySearch(key);

            if(index >= 0)
            {
                _buffer[index].value = value;
                return true;
            }

            return false;
        }

        public bool Remove(TKey key)
        {
            if(_count == 0)
            {
                return false;
            }

            int index = BinarySearch(key);

            if(index >= 0)
            {
                _count--;
                if(_count < index)
                {
                    int length = _capacity - index;
                    Pair* src = _buffer + index + 1;
                    Pair* dst = src + index;
                    Unsafe.CopyBlock(dst, src, (uint)(sizeof(Pair) * length));
                }

                _buffer[_count] = default;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = BinarySearch(key);
            if(index >= 0)
            {
                value = _buffer[index].value;
                return true;
            }

            value = default;
            return false;
        }

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

        public TValue GetValue(TKey key)
        {
            if(!TryGetValue(key, out TValue value))
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

        public bool ContainsKey(TKey key)
        {
            if(_count == 0)
            {
                return false;
            }

            return BinarySearch(key) >= 0;
        }

        public bool ContainsValue(TValue value)
        {
            if(_count == 0)
            {
                return false;
            }

            var comparer = EqualityComparer<TValue>.Default;

            for(int i = 0; i < _count; i++)
            {
                if(comparer.Equals(_buffer[i].value, value))
                {
                    return true;
                }
            }

            return false;
        }

        public void TrimExcess()
        {
            TrimExcess(_count);
        }

        public void TrimExcess(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException($"capacity should be greater than 0: {capacity}", nameof(capacity));
            }

            if(capacity <= _count)
            {
                return;
            }

            Resize(capacity);
        }

        public void EnsureCapacity(int capacity)
        {
            if(capacity <= 0)
            {
                throw new ArgumentException($"capacity should be greater than 0: {capacity}", nameof(capacity));
            }

            if(capacity > _capacity)
            {
                Resize(capacity);
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

            _buffer = GetAllocator()!.Reallocate<Pair>(_buffer, newCapacity);
            _capacity = newCapacity;
        }

        private bool TryInsert(TKey key, TValue value, InsertMode mode)
        {
            if (_capacity == 0)
            {
                return false;
            }

            if(_count == 0 && ((mode == InsertMode.Add || mode == InsertMode.Any)))
            {
                _buffer[_count++] = new Pair(key, value);
                return true;
            }

            int index = BinarySearch(key);

            if(index >= 0 && (mode == InsertMode.Replace || mode == InsertMode.Any))
            {
                _buffer[index].value = value;
                return true;
            }
            if(mode == InsertMode.Replace)
            {
                return false;
            }
            if (index < 0)
            {
                index = ~index + 1;
            }
            if (_count == _capacity)
            {
                Resize();
            }

            int length = _capacity - index;
            Pair* src = _buffer + index;
            Pair* dst = src + 1;
            Unsafe.CopyBlock(dst, src, (uint)(sizeof(Pair) * length));
            _buffer[index] = new Pair(key, value);
            _count++;

            return true;
        }

        private readonly int BinarySearch(TKey key)
        {
            int start = 0;
            int end = _count;

            var comparer = Comparer<TKey>.Default;

            while(start < end)
            {
                int mid = start + (end - start >> 1);
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
            }
        }
    }
}
