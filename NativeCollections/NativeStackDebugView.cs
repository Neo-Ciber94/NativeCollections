using System.Diagnostics;

namespace NativeCollections
{
    internal class NativeStackDebugView<T> where T: unmanaged
    {
        private NativeStack<T> _stack;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[_stack.Length];
                int i = 0;
                foreach (var e in _stack)
                {
                    array[i++] = e;
                }
                return array;
            }
        }

        public NativeStackDebugView(NativeStack<T> stack)
        {
            _stack = stack;
        }
    }
}