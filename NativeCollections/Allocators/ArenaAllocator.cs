using System;
using System.ComponentModel;

namespace NativeCollections.Allocators
{
    unsafe public sealed class ArenaAllocator : Allocator, IDisposable
    {
        private readonly byte* _start;
        private readonly byte* _end;
        private byte* _prevOffset;
        private byte* _offset;

        public ArenaAllocator(int totalBytes) : base(true)
        {
            if (totalBytes <= 0)
                throw new ArgumentException(nameof(totalBytes));

            _start = (byte*)Default.Allocate(totalBytes);
            _end = _start + totalBytes;
            _offset = _start;
        }

        public override unsafe void* Allocate(int elementCount, int elementSize = 1, bool initMemory = true)
        {
            int totalBytes = elementCount * elementSize;
            byte* next = _offset + totalBytes;

            if(next > _end)
            {
                throw new OutOfMemoryException();
            }

            byte* ptr = _offset;
            _prevOffset = _offset;
            _offset = next;
            return ptr;
        }

        public override void* Reallocate(void* pointer, int elementCount, int elementSize = 1, bool initMemory = true)
        {
            if(_prevOffset == pointer)
            {
                int prevBlockSize = (int)(_offset - _prevOffset);
                int totalBytes = (elementCount * elementSize) - prevBlockSize;

                if(totalBytes <= 0)
                {
                    return pointer;
                }
                else
                {
                    byte* next = _offset + totalBytes;

                    if (next > _end)
                    {
                        throw new OutOfMemoryException();
                    }

                    _offset += totalBytes;
                    return _prevOffset;
                }
            }
            else
            {
                return Allocate(elementCount, elementSize, initMemory);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override unsafe void Free(void* pointer) 
        {
            // There is not memory free in arena allocators
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
    }
}
