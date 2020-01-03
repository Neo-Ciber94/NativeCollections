using System.Diagnostics;

namespace NativeCollections
{
    internal class NativeDequeDebugView<T> where T : unmanaged
    {
        private NativeDeque<T> _deque;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[_deque.Length];
                int i = 0;
                foreach (var e in _deque)
                {
                    array[i++] = e;
                }
                return array;
            }
        }

        public NativeDequeDebugView(NativeDeque<T> queue)
        {
            _deque = queue;
        }
    }
}