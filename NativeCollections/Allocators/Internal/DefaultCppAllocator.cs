using System;
using NativeCollections.PInvoke;

namespace NativeCollections.Allocators
{
    public sealed class DefaultCppAllocator : Allocator
    {
        public static readonly DefaultCppAllocator Instance = new DefaultCppAllocator();

        private DefaultCppAllocator() : base(true) { }

        public override unsafe void* Allocate(int elementCount, int elementSize = 1, bool initMemory = true)
        {
            if (elementCount <= 0)
                throw new ArgumentException(elementCount.ToString(), nameof(elementCount));

            if (elementSize <= 0)
                throw new ArgumentException(elementSize.ToString(), nameof(elementSize));

            int bytesCount = elementCount * elementSize;
            CppMemoryFlags flags = initMemory ? CppMemoryFlags.ZERO_MEMORY : CppMemoryFlags.UNITIALIZATED;
            void* block = CppMemory.Malloc((uint)bytesCount, flags);

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
            CppMemoryFlags flags = initMemory ? CppMemoryFlags.ZERO_MEMORY : CppMemoryFlags.UNITIALIZATED;
            void* block = CppMemory.Realloc(pointer, (uint)bytesCount, flags);

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

            CppMemory.Free(pointer);
        }
    }
}
