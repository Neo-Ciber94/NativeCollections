using System;
using NativeCollections.Internal;

namespace NativeCollections.Allocators.Internal
{
    unsafe public sealed class DefaultHeapAllocator : Allocator
    {
        public static readonly DefaultHeapAllocator Instance = new DefaultHeapAllocator();

        private static readonly void* _HeapPointer = Kernel32HeapMemory.GetProcessHeap();

        private DefaultHeapAllocator() : base(true) { }

        public override void* Allocate(int elementCount, int elementSize = 1, bool initMemory = true)
        {
            if (elementCount <= 0)
            {
                throw new ArgumentException(elementCount.ToString(), nameof(elementCount));
            }

            if (elementSize <= 0)
            {
                throw new ArgumentException(elementSize.ToString(), nameof(elementSize));
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
                throw new ArgumentException(elementCount.ToString(), nameof(elementCount));
            }

            if (elementSize <= 0)
            {
                throw new ArgumentException(elementSize.ToString(), nameof(elementSize));
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

        public int SizeOf(void* pointer)
        {
            if (pointer == null)
            {
                throw new ArgumentException("Invalid pointer");
            }

            return (int)Kernel32HeapMemory.HeapSize(_HeapPointer, HeapMemoryFlags.NONE, pointer);
        }
    }
}
