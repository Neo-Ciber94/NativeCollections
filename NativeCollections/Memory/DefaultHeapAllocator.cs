using System;
using NativeCollections.Internal;

namespace NativeCollections.Memory
{
    unsafe internal sealed class DefaultHeapAllocator : Allocator
    {
        public static readonly DefaultHeapAllocator Instance = new DefaultHeapAllocator();

        private static readonly void* _HeapPointer = HeapMemory.GetProcessHeap();

        private DefaultHeapAllocator() { }

        public override unsafe void* Allocate(int bytes, bool initMemory = true)
        {
            if(bytes <= 0)
            {
                throw new ArgumentException(nameof(bytes));
            }

            HeapMemoryFlags flags = initMemory ? HeapMemoryFlags.HEAP_ZERO_MEMORY : HeapMemoryFlags.NONE;
            void* memory = HeapMemory.HeapAlloc(_HeapPointer, flags, (uint)bytes);

            if(memory == null)
            {
                throw new OutOfMemoryException();
            }

            return memory;
        }

        public override unsafe void Free(void* pointer)
        {
            if(HeapMemory.HeapFree(_HeapPointer, HeapMemoryFlags.NONE, pointer) == false)
            {
                throw new InvalidOperationException("Invalid pointer");
            }
        }

        public override unsafe void* ReAllocate(void* pointer, int bytes)
        {
            if (bytes <= 0)
            {
                throw new ArgumentException(nameof(bytes));
            }

            void* memory = HeapMemory.HeapReAlloc(_HeapPointer, HeapMemoryFlags.HEAP_ZERO_MEMORY, pointer, (uint)bytes);

            if (memory == null)
            {
                throw new OutOfMemoryException();
            }

            return memory;
        }
    }
}
