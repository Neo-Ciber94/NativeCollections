using System;
using System.Collections.Generic;
using System.Text;

namespace NativeCollections.Allocators
{
    unsafe public sealed class ArenaAllocator : Allocator, IDisposable
    {
        private readonly byte* _start;
        private readonly byte* _end;
        private byte* _current;

        public ArenaAllocator(int totalBytes) : base(true)
        {
            if (totalBytes <= 0)
                throw new ArgumentException(nameof(totalBytes));

            _start = (byte*)Default.Allocate(totalBytes);
            _end = _start + totalBytes;
            _current = _start;
        }

        public override unsafe void* Allocate(int size, int alignment = 1, bool initMemory = true)
        {
            int totalBytes = size * alignment;
            byte* next = _current + totalBytes;

            if(next > _end)
            {
                throw new OutOfMemoryException();
            }

            byte* cur = _current;
            _current = next;
            return cur;
        }

        public void Dispose()
        {
            Dispose(true);
            Default.Free(_start);
            GC.SuppressFinalize(this);
        }

        ~ArenaAllocator()
        {
            Dispose(false);
        }

        public override unsafe void Free(void* pointer) { }

        public override unsafe void* Reallocate(void* pointer, int size, int alignment = 1, bool initMemory = true)
        {
            throw new InvalidOperationException("Cannot reallocate memory in an Arena allocator");
        }
    }
}
