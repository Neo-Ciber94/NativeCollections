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
    [DebuggerTypeProxy(typeof(NativeMultiValueMapDebugView<,>))]
    unsafe public struct NativeMultiValueMap<TKey, TValue> : INativeContainer<KeyValuePair<TKey, TValue>>, IDisposable where TKey: unmanaged where TValue: unmanaged
    {
        private const int DefaultListCapacity = 10;

        internal NativeMap<TKey, NativeList<TValue>> _map;
        private int _count;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeMultiValueMap{TKey, TValue}"/> struct.
        /// </summary>
        /// <param name="initialSlots">The initial amount of slots for store values.</param>
        public NativeMultiValueMap(int initialSlots) : this(initialSlots, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeMultiValueMap{TKey, TValue}"/> struct.
        /// </summary>
        /// <param name="initialSlots">The initial amount of slots for store values.</param>
        /// <param name="allocator">The allocator.</param>
        /// <exception cref="ArgumentException">If the initialCapacity is negative or zero, or the allocator is no in cache.</exception>
        public NativeMultiValueMap(int initialSlots, Allocator allocator)
        {
            _map = new NativeMap<TKey, NativeList<TValue>>(initialSlots, allocator);
            _count = 0;
        }

        private NativeMultiValueMap(ref NativeMultiValueMap<TKey, TValue> multiValueMap)
        {
            if (!multiValueMap.IsValid)
            {
                throw new ArgumentException("the map is invalid");
            }

            Allocator allocator = multiValueMap.GetAllocator()!;
            int slots = multiValueMap.Slots;
            int sizeOfEntry = sizeof(NativeMap<TKey, NativeList<TValue>>.Entry);
            var buffer = allocator.Allocate<NativeMap<TKey, NativeList<TValue>>.Entry>(slots);
            var source = multiValueMap._map._buffer;
            Unsafe.CopyBlockUnaligned(buffer, source, (uint)(sizeOfEntry * slots));

            for(int i = 0; i < slots; i++)
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
        /// Gets the number of slots used for store the values in the map.
        /// </summary>
        public int Slots => _map.Capacity;

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
        /// Gets the values associated to the given key, or sets the values for the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="NativeSlice{TValue}" />.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The values associated to the key.</returns>
        public NativeSlice<TValue> this[TKey key]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get
            {
                ref NativeList<TValue> list = ref _map.GetValueReference(key);
                return list[..];
            }

            set
            {
                if(_map.TryGetValueReference(key, out var reference))
                {
                    ref NativeList<TValue> list = ref reference.Value;
                    list.Clear();
                    list.AddRange(value.Span);
                }
                else
                {
                    Add(key, value.Span);
                }
            }
        }

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
            if (values.IsEmpty)
                return;

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
        /// Replace all the values of the specified key with the given.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        /// <returns><c>true</c> if the key exists and its values can be replaced, otherwise <c>false</c>.</returns>
        public bool ReplaceValues(TKey key, in Span<TValue> values)
        {
            if (values.IsEmpty)
                return false;

            if (_map.TryGetValueReference(key, out var reference))
            {
                ref NativeList<TValue> list = ref reference.Value;
                list.Clear();
                list.AddRange(values);
                return true;
            }

            return false;
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
        /// Clears the contents of this map.
        /// </summary>
        public void Clear()
        {
            if (_map.IsValid is false)
                return;

            foreach (ref var entry in _map)
            {
                entry.Value.Dispose();
            }

            _map.Clear();
            _count = 0;
        }

        /// <summary>
        /// Gets the values associated to the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A view to the values associated to the key.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeSlice<TValue> GetValues(TKey key)
        {
            return _map.GetValue(key)[..];
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// Copies the content of this map to a <see cref="Span{T}"/>.
        /// an allocated <see cref="NativeArray{T}"/>.
        /// </summary>
        /// <param name="span">The destination span to copy the data.</param>
        /// <param name="index">Start index of the destination where start to copy.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentException">If the span is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the index or count are out of range.</exception>
        public void CopyTo(in Span<KeyValuePair<TKey, TValue>> span, int index, int count)
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

                // TODO: Use KeyValueRef to avoid copy

                TKey key = entry.Key;
                NativeList<TValue> values = entry.Value;

                foreach(ref TValue value in values)
                {
                    if (count == 0)
                    {
                        break;
                    }

                    span[index++] = KeyValuePair.Create(key, value);
                    count--;
                }
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
        public NativeMultiValueMap<TKey, TValue> Clone()
        {
            return _map.IsValid is false? default : new NativeMultiValueMap<TKey, TValue>(ref this);
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
                this = default;
            }
        }

        /// <summary>
        /// Gets an enumerator over the elements of this map.
        /// </summary>
        /// <returns>A enumerator over the elements of the map.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator()
        {
            return new Enumerator(_map.GetEnumerator());
        }

        /// <summary>
        /// An enumerator over the key and values of the map.
        /// </summary>
        public ref struct Enumerator
        {
            private struct KeyValueRef
            {
                public TKey key;
                public NativeList<TValue> value;
            }

            private NativeMap<TKey, NativeList<TValue>>.Enumerator _enumerator;
            private bool _hasNext;

            internal Enumerator(NativeMap<TKey, NativeList<TValue>>.Enumerator enumerator)
            {
                _enumerator = enumerator;
                _hasNext = false;
            }

            /// <summary>
            /// Gets the current value.
            /// </summary>
            public KeyValueCollection Current
            {
                get
                {
                    if (!_hasNext)
                    {
                        throw new InvalidOperationException("No more values");
                    }

                    ref KeyValuePair<TKey, NativeList<TValue>> keyValuePair = ref _enumerator.Current;
                    ref KeyValueRef keyValue = ref Unsafe.As<KeyValuePair<TKey, NativeList<TValue>>, KeyValueRef>(ref keyValuePair);
                    return new KeyValueCollection(ref keyValue.key, keyValue.value[..]);
                }
            }

            /// <summary>
            /// Move to the next value.
            /// </summary>
            public bool MoveNext()
            {
                _hasNext = _enumerator.MoveNext();
                return _hasNext;
            }

            /// <summary>
            /// Dispose this enumerator.
            /// </summary>
            public void Dispose()
            {
                _enumerator.Dispose();
            }

            /// <summary>
            /// Resets this enumerator.
            /// </summary>
            public void Reset()
            {
                _enumerator.Reset();
            }
        }

        /// <summary>
        /// Represents a key and a collection of the values associated to it.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the values.</typeparam>
        unsafe public readonly ref struct KeyValueCollection
        {
            private readonly TKey* _key;
            private readonly NativeSlice<TValue> _values;

            /// <summary>
            /// Initializes a new instance of the <see cref="KeyValueCollection{TKey, TValue}" /> struct.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="values">The values.</param>
            public KeyValueCollection(ref TKey key, NativeSlice<TValue> values)
            {
                _key = (TKey*)Unsafe.AsPointer(ref key);
                _values = values;
            }

            /// <summary>
            /// Gets a readonly reference to the key.
            /// </summary>
            public readonly ref TKey Key => ref *_key;

            /// <summary>
            /// Gets the values associated to the key.
            /// </summary>
            public NativeSlice<TValue> Values => _values;

            [EditorBrowsable(EditorBrowsableState.Never)]
            public void Deconstruct(out TKey key, out NativeSlice<TValue> slice)
            {
                key = *_key;
                slice = _values;
            }

            /// <summary>
            /// Gets a string representation of this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String" /> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                StringBuilder sb = StringBuilderCache.Acquire();
                sb.Append('[');
                sb.Append(_key->ToString());
                sb.Append(", ");
                sb.Append(Values.ToString());
                sb.Append(']');
                return StringBuilderCache.ToStringAndRelease(ref sb!);
            }
        }
    }
}