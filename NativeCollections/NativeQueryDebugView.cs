using System.Diagnostics;

namespace NativeCollections
{
    internal sealed class NativeQueryDebugView<T> where T: unmanaged
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items { get; }

        public NativeQueryDebugView(NativeQuery<T> query)
        {
            T[] array = new T[query.Length];
            int i = 0;
            foreach(var e in query)
            {
                array[i++] = e;
            }

            Items = array;
        }
    }
}