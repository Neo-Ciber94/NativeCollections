using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using NativeCollections.Allocators;
using NativeCollections.Utility;

namespace NativeCollections
{
    /// <summary>
    /// Represents a collection of key-values where each key is associated to one or more values.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="INativeContainer{KeyValuePair{TKey, NativeArray{TValue}}}" />
    /// <seealso cref="IDisposable" />
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(MultiValueNativeMapDebugView<,>))]
    unsafe public struct MultiValueNativeMap<TKey, TValue> : INativeContainer<KeyValuePair<TKey, NativeArray<TValue>>>, IDisposable where TKey: unmanaged where TValue: unmanaged
    {
        private const int DefaultListCapacity = 10;

        internal NativeMap<TKey, NativeList<TValue>> _map;
        private int _count;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiValueNativeMap{TKey, TValue}"/> struct.
        /// </summary>
        /// <param name="initialSlots">The initial amount of slots for store values.</param>
        public MultiValueNativeMap(int initialSlots) : this(initialSlots, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiValueNativeMap{TKey, TValue}"/> struct.
        /// </summary>
        /// <param name="initialSlots">The initial amount of slots for store values.</param>
        /// <param name="allocator">The allocator.</param>
        /// <exception cref="ArgumentException">If the initialCapacity is negative or zero, or the allocator is no in cache.</exception>
        public MultiValueNativeMap(int initialSlots, Allocator allocator)
        {
            _map = new NativeMap<TKey, NativeList<TValue>>(initialSlots, allocator);
            _count = 0;
        }

        private MultiValueNativeMap(ref MultiValueNativeMap<TKey, TValue> multiValueMap)
        {
            if (!multiValueMap.IsValid)
            {
                throw new ArgumentException("the map is invalid");
            }

            Allocator allocator = multiValueMap.GetAllocator()!;
            int length = multiValueMap.Length;
            int sizeOfEntry = sizeof(NativeMap<TKey, NativeList<TValue>>.Entry);
            var buffer = allocator.Allocate<NativeMap<TKey, NativeList<TValue>>.Entry>(length);
            var source = multiValueMap._map._buffer;
            Unsafe.CopyBlockUnaligned(buffer, source, (uint)(sizeOfEntry * length));

            for(int i = 0; i < length; i++)
            {
                ref var entry = ref buffer[i];
                if(entry.hashCode >= 0)
                {
                    entry.value = entry.value.Clone();
                }
            }

            _map = new NativeMap<TKey, NativeList<TValue>>(ref multiValueMap._map, buffer);
            _count = multiValueMap._count;
        }

        /// <summary>
        /// Gets the number of elements in the map.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length => _count;

        /// <summary>
        /// Checks if the map is allocated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this map is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid => _map.IsValid;

        /// <summary>
        /// Gets a value indicating whether this map is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => _map.IsEmpty;

        /// <summary>
        /// Gets the allocator.
        /// </summary>
        /// <returns>The allocator used for this map.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Allocator? GetAllocator()
        {
            return _map.GetAllocator();
        }

        /// <summary>
        /// Adds the specified key and value to the map.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value)
        {
            if(_map.TryGetValueReference(key, out ByReference<NativeList<TValue>> reference))
            {
                reference.Value.Add(value);
            }
            else
            {
                var list = new NativeList<TValue>(DefaultListCapacity);
                list.Add(value);
                _map.Add(key, list);
            }

            _count++;
        }

        /// <summary>
        /// Adds the specified key and values to the map.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        public void Add(TKey key, in Span<TValue> values)
        {
            if(_map.TryGetValueReference(key, out ByReference<NativeList<TValue>> reference))
            {
                reference.Value.AddRange(values);
            }
            else
            {
                var list = new NativeList<TValue>(values.Length);
                list.AddRange(values);
                _map.Add(key, list);
            }

            _count += values.Length;
        }

        /// <summary>
        /// Removes the specified key of the maps and all the values associated to it.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the key was removed, otherwise false.</returns>
        public bool Remove(TKey key)
        {
            if(_map.TryGetValueReference(key, out ByReference<NativeList<TValue>> reference))
            {
                ref NativeList<TValue> list = ref reference.Value;
                _count -= list.Length;
                list.Dispose();
                _map.Remove(key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the specified value if its associated to the key, and remove the key if no have any value associated to it.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value was removed.</returns>
        public bool RemoveValue(TKey key, TValue value)
        {
            if(_map.TryGetValueReference(key, out ByReference<NativeList<TValue>> reference))
            {
                ref NativeList<TValue> list = ref reference.Value;
                if (list.Remove(value))
                {
                    _count--;
                    if(list.Length == 0)
                    {
                        list.Dispose();
                        _map.Remove(key);
                    }
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attemps to get the values associated to the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        /// <returns><c>true</c> if the key exists.</returns>
        public bool TryGetValues(TKey key, out NativeSlice<TValue> values)
        {
            if(_map.TryGetValue(key, out NativeList<TValue> list))
            {
                values = list[..];
                return true;
            }

            values = default;
            return false;
        }

        /// <summary>
        /// Determines whether the map contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the map contains the specified key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            return _map.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the key contains the specified value associated to it.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified key have the given value associated to it; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsValue(TKey key, TValue value)
        {
            if(_map.TryGetValueReference(key, out ByReference<NativeList<TValue>> reference))
            {
                return reference.Value.Contains(value);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the map contains the specified value contains value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the map contains the specified value contains value; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsValue(TValue value)
        {
            foreach (ref var entry in _map)
            {
                NativeList<TValue> list = entry.Value;

                if (list.Contains(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Copies the content of this map to a <see cref="Span{T}"/> where each <see cref="KeyValuePair{TKey, TValue}"/> in the span will have
        /// an allocated <see cref="NativeArray{T}"/>.
        /// </summary>
        /// <param name="span">The destination span to copy the data.</param>
        /// <param name="index">Start index of the destination where start to copy.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentException">If the span is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the index or count are out of range.</exception>
        public void CopyTo(in Span<KeyValuePair<TKey, NativeArray<TValue>>> span, int index, int count)
        {
            if (span.IsEmpty)
                throw new ArgumentException("Span is empty", nameof(span));

            if (index < 0 || index > span.Length)
                throw new ArgumentOutOfRangeException(nameof(index), index.ToString());

            if (count < 0 || count > Length)
                throw new ArgumentOutOfRangeException(nameof(count), count.ToString());

            foreach (ref var entry in _map)
            {
                if (count == 0)
                {
                    break;
                }

                TKey key = entry.Key;
                NativeArray<TValue> array = entry.Value.ToNativeArray();

                span[index++] = KeyValuePair.Create(key, array);
                count--;
            }
        }

        /// <summary>
        /// Gets a string representation of the key-values of this map.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _map.ToString();
        }

        /// <summary>
        /// Gets a deep clone of this instance.
        /// </summary>
        /// <returns>A copy of this instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MultiValueNativeMap<TKey, TValue> Clone()
        {
            if (!_map.IsValid)
            {
                return default;
            }

            return new MultiValueNativeMap<TKey, TValue>(ref this);
        }

        /// <summary>
        /// Releases all the resources used for this map.
        /// </summary>
        public void Dispose()
        {
            if (_map.IsValid)
            {
                foreach (ref var entry in _map)
                {
                    NativeList<TValue> list = entry.Value;

                    if (list.IsValid)
                    {
                        list.Dispose();
                    }
                }

                _map.Dispose();
                _map = default;
            }
        }

        /// <summary>
        /// Gets an enumerator over the elements of this map.
        /// </summary>
        /// <returns>A enumerator over the elements of the map.</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(_map.GetEnumerator());
        }

        public ref struct KeyAndValues
        {
            public TKey Key { get; }
            public NativeSlice<TValue> Values { get; }

            public KeyAndValues(TKey key, NativeSlice<TValue> values)
            {
                Key = key;
                Values = values;
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            public void Deconstruct(out TKey key, out NativeSlice<TValue> values)
            {
                key = Key;
                values = Values;
            }

            public override string ToString()
            {
                StringBuilder sb = StringBuilderCache.Acquire();
                sb.Append('(');
                sb.Append(Key.ToString());
                sb.Append(", ");
                sb.Append(Values.ToString());
                sb.Append(')');
                return StringBuilderCache.ToStringAndRelease(ref sb!);
            }
        }

        public ref struct Enumerator
        {
            private NativeMap<TKey, NativeList<TValue>>.Enumerator _enumerator;
            private bool _hasNext;

            internal Enumerator(NativeMap<TKey, NativeList<TValue>>.Enumerator enumerator)
            {
                _enumerator = enumerator;
                _hasNext = false;
            }

            public KeyAndValues Current
            {
                get
                {
                    if (!_hasNext)
                        throw new InvalidOperationException("No more values");

                    ref KeyValuePair<TKey, NativeList<TValue>> pair = ref _enumerator.Current;
                    return new KeyAndValues(pair.Key, pair.Value[..]);
                }
            }

            public bool MoveNext()
            {
                _hasNext = _enumerator.MoveNext();
                return _hasNext;
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }
        }
    }

    unsafe public readonly ref struct KeyAndView<TKey, TValue> where TKey: unmanaged where TValue: unmanaged
    {
        private readonly TKey* _key;
        private readonly NativeSlice<TValue> _slice;

        internal KeyAndView(ref TKey key, NativeSlice<TValue> slice)
        {
            _key = (TKey*)Unsafe.AsPointer(ref key);
            _slice = slice;
        }

        public readonly ref TKey Key => ref * _key;
        public NativeSlice<TValue> Values => _slice;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out TKey key, out NativeSlice<TValue> slice)
        {
            key = *_key;
            slice = _slice;
        }
    }
}