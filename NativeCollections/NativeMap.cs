using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NativeCollections.Memory;

namespace NativeCollections
{
    unsafe public struct NativeMap<TKey, TValue> : IDisposable where TKey : unmanaged where TValue : unmanaged
    {
        public enum InsertMode { Any, Add, Replace }

        private struct Entry
        {
            public TKey key;
            public TValue value;
            public int hashCode;
            public int next;
            public int bucket;
        }

        private Entry* _buffer;
        private int _count;
        private int _capacity;
        private int _freeList;
        private int _freeCount;

        public NativeMap(int initialCapacity)
        {
            if (initialCapacity <= 0)
                throw new ArgumentException($"initialCapacity should be greater than 0: {initialCapacity}");

            _buffer = Allocator.Default.Allocate<Entry>(initialCapacity);
            _capacity = initialCapacity;
            _count = 0;
            _freeList = -1;
            _freeCount = 0;

            for (int i = 0; i < _capacity; i++)
            {
                _buffer[i].bucket = -1;
            }
        }

        public int Length => _count;

        public int Capacity => _capacity;

        public bool IsValid => _buffer != null;

        public bool IsEmpty => _count == 0;

        public void Add(TKey key, TValue value)
        {
            if (!TryInsert(key, value, InsertMode.Add))
                throw new InvalidOperationException($"Duplicated key: {key}");
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
            var hashCode = GetHash(ref key);
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
                    _count--;
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

        public bool ContainsKey(TKey key)
        {
            return FindEntry(key) >= 0;
        }

        public bool ContainsValue(TValue value)
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

        private bool TryInsert(TKey key, TValue value, InsertMode mode)
        {
            var comparer = EqualityComparer<TKey>.Default;
            var hashCode = GetHash(ref key);
            var bucket = GetBucket(hashCode, _capacity);
            var index = _buffer[bucket].bucket;

            while (index >= 0)
            {
                ref Entry entry = ref _buffer[index];
                if ((mode == InsertMode.Replace) && comparer.Equals(entry.key, key) && hashCode == entry.hashCode)
                {
                    entry.value = value;
                    return true;
                }

                if (mode == InsertMode.Add && comparer.Equals(entry.key, key) && hashCode == entry.hashCode)
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
                index = _count;
            }

            _buffer[index].key = key;
            _buffer[index].value = value;
            _buffer[index].hashCode = hashCode;
            _buffer[index].next = _buffer[bucket].bucket;
            _buffer[bucket].bucket = index;
            _count++;

            if (_count == _capacity)
            {
                Resize();
            }

            return true;
        }

        private int FindEntry(TKey key)
        {
            var comparer = EqualityComparer<TKey>.Default;
            var hashCode = GetHash(ref key);
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

        private void Resize()
        {
            SetCapacity(_capacity * 2);
        }

        private void SetCapacity(int newCapacity)
        {
            var newBuffer = Allocator.Default.Allocate<Entry>(newCapacity);
            Unsafe.CopyBlock(newBuffer, _buffer, (uint)(sizeof(Entry) * _count));

            for (int i = _capacity; i < newCapacity; i++)
            {
                newBuffer[i].bucket = -1;
            }

            for (int i = 0; i < _count; i++)
            {
                if (newBuffer[i].hashCode >= 0)
                {
                    int hashCode = GetHash(ref newBuffer[i].key);
                    int bucket = GetBucket(hashCode, newCapacity);
                    newBuffer[i].next = newBuffer[bucket].bucket;
                    newBuffer[bucket].bucket = i;
                }
            }

            Allocator.Default.Free(_buffer);

            _capacity = newCapacity;
            _buffer = newBuffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetHash(ref TKey key)
        {
            return key.GetHashCode() & int.MaxValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetBucket(int hashCode, int capacity)
        {
            return hashCode % capacity;
        }

        public void Dispose()
        {
            if (_buffer == null)
                return;

            Allocator.Default.Free(_buffer);
            _buffer = null;
            _capacity = 0;
            _count = 0;
            _freeCount = 0;
            _freeList = 0;
        }
    }
}
