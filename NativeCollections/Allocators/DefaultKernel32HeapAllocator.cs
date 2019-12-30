using System;
using NativeCollections.Internal;

namespace NativeCollections.Allocators
{
    unsafe internal sealed class DefaultKernel32HeapAllocator : Allocator
    {
        public static readonly DefaultKernel32HeapAllocator Instance = new DefaultKernel32HeapAllocator();

        private static readonly void* _HeapPointer = Kernel32HeapMemory.GetProcessHeap();

        private DefaultKernel32HeapAllocator() : base(true) { }

        public override unsafe void* Allocate(int size, int alignment = 1, bool initMemory = true)
        {
            if(size <= 0)
            {
                throw new ArgumentException(nameof(size));
            }

            if (alignment <= 0)
            {
                throw new ArgumentException(nameof(alignment));
            }

            HeapMemoryFlags flags = initMemory ? HeapMemoryFlags.HEAP_ZERO_MEMORY : HeapMemoryFlags.NONE;
            void* memory = Kernel32HeapMemory.HeapAlloc(_HeapPointer, flags, (uint)(size * alignment));

            if(memory == null)
            {
                throw new OutOfMemoryException();
            }

            return memory;
        }

        public override unsafe void* Reallocate(void* pointer, int size, int alignment = 1, bool initMemory = true)
        {
            if (size <= 0)
            {
                throw new ArgumentException(nameof(size));
            }

            if(alignment <= 0)
            {
                throw new ArgumentException(nameof(alignment));
            }

            HeapMemoryFlags flags = initMemory ? HeapMemoryFlags.HEAP_ZERO_MEMORY : HeapMemoryFlags.NONE;
            void* memory = Kernel32HeapMemory.HeapReAlloc(_HeapPointer, flags, pointer, (uint)(size * alignment));

            if (memory == null)
            {
                throw new OutOfMemoryException();
            }

            return memory;
        }


        public override unsafe void Free(void* pointer)
        {
            if(Kernel32HeapMemory.HeapFree(_HeapPointer, HeapMemoryFlags.NONE, pointer) == false)
            {
                throw new InvalidOperationException("Invalid pointer");
            }
        }
    }
}
