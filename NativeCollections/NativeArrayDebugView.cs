using System.Diagnostics;

namespace NativeCollections
{
    internal sealed class NativeArrayDebugView<T> where T: unmanaged
    {
        private NativeArray<T> _array;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[_array.Length];
                for(int i = 0; i < array.Length; i++)
                {
                    array[i] = _array[i];
                }
                return array;
            }
        }

        public NativeArrayDebugView(NativeArray<T> array)
        {
            _array = array;
        }
    }
}
