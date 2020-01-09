using System.Diagnostics;

namespace NativeCollections.Buffers
{
    internal class NativeBufferDebugView
    {
        private NativeBuffer _buffer;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public byte[] Items => ToArray();

        public NativeBufferDebugView(NativeBuffer buffer)
        {
            _buffer = buffer;
        }

        unsafe private byte[] ToArray()
        {
            byte[] array = new byte[_buffer.TotalBytes];
            for(int i = 0; i < array.Length; i++)
            {
                array[i] = _buffer._buffer[i];
            }
            return array;
        }
    }
}