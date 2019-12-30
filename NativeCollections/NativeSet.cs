using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using NativeCollections.Allocators;

namespace NativeCollections
{
    unsafe public struct NativeSet<T> : IDisposable where T: unmanaged
    {
        internal struct Entry
        {
            public T value;
            public int hashCode;
            public int next;
            public int bucket;
        }

        public ref struct Enumerator
        {
            private Entry* _entries;
            private int _count;
            private int _index;

            public Enumerator(ref NativeSet<T> set)
            {
                _entries = set._buffer;
                _count = set._count;
                _index = -1;
            }

            public readonly ref T Current
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

        private Entry* _buffer;
        private int _capacity;
        private int _count;
        private int _freeCount;
        private int _freeList;
        private int _allocatorID;

        public NativeSet(int initialCapacity) : this(initialCapacity, Allocator.Default) { }

        public NativeSet(int initialCapacity, Allocator allocator)
        {
            if (initialCapacity <= 0)
                throw new ArgumentException("initialCapacity should be greater than 0.", nameof(initialCapacity));

            _buffer = (Entry*)allocator.Allocate(initialCapacity, sizeof(Entry));
            _capacity = initialCapacity;
            _count = 0;
            _freeList = -1;
            _freeCount = 0;
            _allocatorID = allocator.ID;

            Initializate();
        }

        public NativeSet(Span<T> elements) : this(elements, Allocator.Default) { }

        public NativeSet(Span<T> elements, Allocator allocator)
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
                    Add(e);
                }
            }
        }

        public int Length => _count - _freeCount;

        public int Capacity => _capacity;

        public bool IsEmpty => _count == 0;

        public bool IsValid => _buffer != null;

        public Allocator? GetAllocator()
        {
            return Allocator.GetAllocatorByID(_allocatorID);
        }

        public bool Add(T value)
        {
            return AddIfAbsent(value);
        }

        public int AddRange(Span<T> elements)
        {
            if (elements.IsEmpty)
                return 0;

            int count = 0;
            foreach(var e in elements)
            {
                if(AddIfAbsent(e))
                {
                    count++;
                }
            }

            return count;
        }

        public bool Remove(T value)
        {
            if (_count == 0)
                return false;

            var comparer = EqualityComparer<T>.Default;
            int hashCode = GetHash(value);
            int bucket = GetBucket(hashCode, _capacity);
            int index = _buffer[bucket].next;
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

        public bool Contains(T value)
        {
            if (_count == 0)
                return false;

            var comparer = EqualityComparer<T>.Default;
            int hashCode = GetHash(value);
            int bucket = GetBucket(hashCode, _capacity);
            int index = _buffer[bucket].next;

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

        public Enumerator GetEnumerator()
        {
            return new Enumerator(ref this);
        }

        public void TrimExcess()
        {
            TrimExcess(Length);
        }

        public void TrimExcess(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException($"capacity must be greater than 0: {capacity}", nameof(capacity));
            }

            if(capacity <= Length)
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

            _freeCount = 0;
            _freeList = -1;
            _count = count;
            _capacity = capacity;
        }

        public void EnsureCapacity(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException($"capacity must be greater than 0: {capacity}", nameof(capacity));
            }

            if(capacity > _capacity)
            {
                Resize(capacity);
            }
        }

        private void Initializate()
        {
            for(int i = 0; i < _capacity; i++)
            {
                _buffer[i].bucket = -1;
            }
        }

        private bool AddIfAbsent(T value)
        {
            if (_count == 0)
                return false; 

            var comparer = EqualityComparer<T>.Default;
            int hashCode = GetHash(value);
            int bucket = GetBucket(hashCode, _capacity);
            int index = _buffer[bucket].next;

            while(index >= 0)
            {
                ref Entry entry = ref _buffer[index];
                if (comparer.Equals(entry.value, value) && hashCode == entry.hashCode)
                {
                    return false;
                }
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
            _buffer[bucket].bucket = _count;
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

        public void Dispose()
        {
            if (_buffer == null)
                return;

            if (Allocator.IsCached(_allocatorID))
            {
                GetAllocator()!.Free(_buffer);
                _buffer = null;
                _count = 0;
                _capacity = 0;
                _freeCount = 0;
                _freeList = 0;
            }
        }
    }
}
