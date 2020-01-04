using System.Collections.Generic;
using System.Diagnostics;

namespace NativeCollections
{
    internal class NativeSortedMapDebugView<TKey, TValue> where TKey: unmanaged where TValue: unmanaged
    {
        private NativeSortedMap<TKey, TValue> _map;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<TKey, TValue>[] Items
        {
            get
            {
                KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[_map.Length];
                int i = 0;
                foreach (var e in _map)
                {
                    array[i++] = e;
                }
                return array;
            }
        }

        public NativeSortedMapDebugView(NativeSortedMap<TKey, TValue> map)
        {
            _map = map;
        }
    }
}