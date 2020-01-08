using System;
using System.Collections.Generic;
using System.Text;
using NativeCollections.Allocators;

namespace NativeCollections
{
    unsafe public struct MultiValueNativeMap<TKey, TValue> : IDisposable where TKey: unmanaged where TValue: unmanaged
    {
        private const int DefaultListCapacity = 10;

        private NativeMap<TKey, NativeList<TValue>> _map;

        public MultiValueNativeMap(int initialCapacity) : this(initialCapacity, Allocator.Default) { }

        public MultiValueNativeMap(int initialCapacity, Allocator allocator)
        {
            _map = new NativeMap<TKey, NativeList<TValue>>(initialCapacity, allocator);
        }

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
        }

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
        }

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

        public void Dispose()
        {
            if (_map.IsValid)
            {
                foreach(ref var entry in _map)
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
    }
}