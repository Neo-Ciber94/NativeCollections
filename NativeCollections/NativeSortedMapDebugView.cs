using System.Collections.Generic;
using System.Diagnostics;

namespace NativeCollections
{
    internal sealed class NativeSortedMapDebugView<TKey, TValue> where TKey: unmanaged where TValue: unmanaged
    {
        private NativeSortedMap<TKey, TValue> _map;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<TKey, TValue>[] Items => _map.ToArray();

        public NativeSortedMapDebugView(NativeSortedMap<TKey, TValue> map)
        {
            _map = map;
        }
    }
}