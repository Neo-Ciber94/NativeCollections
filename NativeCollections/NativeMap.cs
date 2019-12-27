﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NativeCollections.Memory;
using NativeCollections.Utility;

namespace NativeCollections
{
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeMapDebugView<,>))]
    unsafe public struct NativeMap<TKey, TValue> : IDisposable where TKey : unmanaged where TValue : unmanaged
    {
        public enum InsertMode { Any, Add, Replace }

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

        internal Entry* _buffer;
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

        public NativeMap(Span<(TKey, TValue)> elements)
        {
            if (elements.IsEmpty)
            {
                this = default;
            }
            else
            {
                _buffer = Allocator.Default.Allocate<Entry>(elements.Length);
                _capacity = elements.Length;
                _count = 0;
                _freeList = -1;
                _freeCount = 0;

                for (int i = 0; i < _capacity; i++)
                {
                    _buffer[i].bucket = -1;
                }

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
            if(_count == 0)
            {
                return Array.Empty<KeyValuePair<TKey, TValue>>();
            }

            KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[Length];
            //CopyTo(array, array.Length);

            int j = 0;
            for(int i = 0; i < _count; i++)
            {
                ref Entry entry = ref _buffer[i];
                if(entry.hashCode >= 0)
                {
                    array[j++] = new KeyValuePair<TKey, TValue>(entry.key, entry.value);
                }
            }
            return array;
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
                if ((mode == InsertMode.Replace || mode == InsertMode.Any) && comparer.Equals(entry.key, key) && hashCode == entry.hashCode)
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

            // Key not fount
            if(mode == InsertMode.Replace)
            {
                return false;
            }
            else
            {
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

        private void Resize()
        {
            SetCapacity(_capacity * 2);
        }

        private void SetCapacity(int newCapacity)
        {
            var newBuffer = Allocator.Default.Allocate<Entry>(newCapacity);
            Unsafe.CopyBlock(newBuffer, _buffer, (uint)(sizeof(Entry) * _count));

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

            Allocator.Default.Free(_buffer);
            _capacity = newCapacity;
            _buffer = newBuffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly int GetHash(in TKey key)
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

        public override string ToString()
        {
            if (_count == 0)
                return "[]";

            // TODO: Reduce StringBuilderCache calls overhead by implementing ToString() for (1..10) elements

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
                while(i < _count)
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
}
