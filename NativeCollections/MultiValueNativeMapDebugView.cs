using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NativeCollections
{
    internal sealed class MultiValueNativeMapDebugView<TKey, TValue> where TKey: unmanaged where TValue: unmanaged
    {
        private readonly NativeMap<TKey, NativeList<TValue>> _map;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<TKey, TValue[]>[] Items
        {
            get
            {
                return _map.ToArray()
                    .Select(e => KeyValuePair.Create(e.Key, ToArraySlow(e.Value)))
                    .ToArray();
            }
        }

        public MultiValueNativeMapDebugView(MultiValueNativeMap<TKey, TValue> map)
        {
            _map = map._map;
        }

        private static T[] ToArraySlow<T>(NativeList<T> list) where T: unmanaged
        {
            T[] array = new T[list.Length];
            for(int i = 0; i < array.Length; ++i)
            {
                array[i] = list[i];
            }
            return array;
        }
    }
}