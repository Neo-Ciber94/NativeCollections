using System;
using NativeCollections.PInvoke;

namespace NativeCollections.Allocators
{
    public sealed class DefaultCAllocator : Allocator
    {
        public static readonly DefaultCAllocator Instance = new DefaultCAllocator();

        private DefaultCAllocator() : base(true) { }

        public override unsafe void* Allocate(int elementCount, int elementSize = 1, bool initMemory = true)
        {
            if (elementCount <= 0)
                throw new ArgumentException(elementCount.ToString(), nameof(elementCount));

            if (elementSize <= 0)
                throw new ArgumentException(elementSize.ToString(), nameof(elementSize));

            int bytesCount = elementCount * elementSize;
            CMemoryFlags flags = initMemory ? CMemoryFlags.ZERO_MEMORY : CMemoryFlags.UNITIALIZATED;
            void* block = CMemory.Malloc((uint)bytesCount, flags);

            if(block == null)
            {
                throw new OutOfMemoryException();
            }

            return block;
        }

        public override unsafe void* Reallocate(void* pointer, int elementCount, int elementSize = 1, bool initMemory = true)
        {
            if (elementCount <= 0)
                throw new ArgumentException(elementCount.ToString(), nameof(elementCount));

            if (elementSize <= 0)
                throw new ArgumentException(elementSize.ToString(), nameof(elementSize));

            if (pointer == null)
                throw new ArgumentException("Pointer is null");

            int bytesCount = elementCount * elementSize;
            CMemoryFlags flags = initMemory ? CMemoryFlags.ZERO_MEMORY : CMemoryFlags.UNITIALIZATED;
            void* block = CMemory.Realloc(pointer, (uint)bytesCount, flags);

            if (block == null)
            {
                throw new OutOfMemoryException();
            }

            return block;
        }

        public override unsafe void Free(void* pointer)
        {
            if (pointer == null)
                throw new ArgumentException("Pointer is null");

            CMemory.Free(pointer);
        }
    }
}
