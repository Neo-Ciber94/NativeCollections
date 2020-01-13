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
            var enumerator = query.GetEnumerator(false);
            int i = 0;

            while (enumerator.MoveNext())
            {
                array[i++] = enumerator.Current;
            }

            Items = array;
        }
    }
}