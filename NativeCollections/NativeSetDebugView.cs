using System.Diagnostics;

namespace NativeCollections
{
    internal class NativeSetDebugView<T> where T: unmanaged
    {
        private NativeSet<T> _set;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[_set.Length];
                int i = 0;
                foreach (var e in _set)
                {
                    array[i++] = e;
                }
                return array;
            }
        }

        public NativeSetDebugView(NativeSet<T> set)
        {
            _set = set;
        }
    }
}