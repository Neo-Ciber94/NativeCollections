using System.Diagnostics;

namespace NativeCollections
{
    internal class NativeSliceDebugView<T>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items { get; }

        public NativeSliceDebugView(NativeSlice<T> slice)
        {
            Items = slice.ToArray();
        }
    }
}