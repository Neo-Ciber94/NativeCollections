using System;
using NativeCollections.Internal;

namespace NativeCollections.Allocators.Internal
{
    unsafe public sealed class DefaultKernel32HeapAllocator : Allocator
    {
        public static readonly DefaultKernel32HeapAllocator Instance = new DefaultKernel32HeapAllocator();

        private static readonly void* _HeapPointer = Kernel32HeapMemory.GetProcessHeap();

        private DefaultKernel32HeapAllocator() : base(true) { }

        public override void* Allocate(int elementCount, int elementSize = 1, bool initMemory = true)
        {
            if(elementCount <= 0)
            {
                throw new ArgumentException(nameof(elementCount));
            }

            if (elementSize <= 0)
            {
                throw new ArgumentException(nameof(elementSize));
            }

            HeapMemoryFlags flags = initMemory ? HeapMemoryFlags.HEAP_ZERO_MEMORY : HeapMemoryFlags.NONE;
            void* memory = Kernel32HeapMemory.HeapAlloc(_HeapPointer, flags, (uint)(elementCount * elementSize));

            if(memory == null)
            {
                throw new OutOfMemoryException();
            }

            return memory;
        }

        public override void* Reallocate(void* pointer, int elementCount, int elementSize = 1, bool initMemory = true)
        {
            if (elementCount <= 0)
            {
                throw new ArgumentException(nameof(elementCount));
            }

            if(elementSize <= 0)
            {
                throw new ArgumentException(nameof(elementSize));
            }

            HeapMemoryFlags flags = initMemory ? HeapMemoryFlags.HEAP_ZERO_MEMORY : HeapMemoryFlags.NONE;
            void* memory = Kernel32HeapMemory.HeapReAlloc(_HeapPointer, flags, pointer, (uint)(elementCount * elementSize));

            if (memory == null)
            {
                throw new OutOfMemoryException();
            }

            return memory;
        }

        public override void Free(void* pointer)
        {
            if (Kernel32HeapMemory.HeapFree(_HeapPointer, HeapMemoryFlags.NONE, pointer) == false)
            {
                throw new InvalidOperationException("Invalid pointer");
            }
        }
    }
}
