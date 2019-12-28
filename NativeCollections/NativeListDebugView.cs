using System.Diagnostics;

namespace NativeCollections
{
    internal sealed class NativeListDebugView<T> where T: unmanaged
    {
        private NativeList<T> _list;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[_list.Length];
                for(int i = 0; i < array.Length; i++)
                {
                    array[i] = _list[i];
                }
                return array;
            }
        }

        public NativeListDebugView(NativeList<T> list)
        {
            _list = list;
        }
    }
}
