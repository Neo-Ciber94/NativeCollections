using System.Diagnostics;

namespace NativeCollections
{
    internal class NativeQueueDebugView<T> where T: unmanaged
    {
        private NativeQueue<T> _queue;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[_queue.Length];
                int i = 0;
                foreach(var e in _queue)
                {
                    array[i++] = e;
                }
                return array;
            }
        }

        public NativeQueueDebugView(NativeQueue<T> queue)
        {
            _queue = queue;
        }
    }
}