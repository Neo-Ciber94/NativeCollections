using System.Collections.Generic;
using System.Diagnostics;

namespace NativeCollections
{
    internal sealed class NativeMapDebugView<TKey, TValue> where TKey: unmanaged where TValue: unmanaged
    {
        private readonly NativeMap<TKey, TValue> _map;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<TKey, TValue>[] Items => _map.ToArray();

        public NativeMapDebugView(NativeMap<TKey, TValue> map)
        {
            _map = map;
        }
    }
}
