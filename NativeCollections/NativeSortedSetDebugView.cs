using System.Diagnostics;

namespace NativeCollections
{
    internal sealed class NativeSortedSetDebugView<T> where T: unmanaged
    {
        private NativeSortedSet<T> _set;

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

        public NativeSortedSetDebugView(NativeSortedSet<T> set)
        {
            _set = set;
        }
    }
}