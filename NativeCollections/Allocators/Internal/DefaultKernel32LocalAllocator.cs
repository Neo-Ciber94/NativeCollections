using System;
using System.Runtime.InteropServices;
using NativeCollections.Internal;

namespace NativeCollections.Allocators.Internal
{
    unsafe public sealed class DefaultKernel32LocalAllocator : Allocator
    {
        public static readonly DefaultKernel32LocalAllocator Instance = new DefaultKernel32LocalAllocator();

        private DefaultKernel32LocalAllocator() : base(true) { }

        public override unsafe void* Allocate(int elementCount, int elementSize = 1, bool initMemory = true)
        {
            if (elementCount <= 0)
            {
                throw new ArgumentException(nameof(elementCount));
            }

            if(elementSize <= 0)
            {
                throw new ArgumentException(nameof(elementSize));
            }

            LocalMemoryFlags flags = initMemory? LocalMemoryFlags.LPTR: LocalMemoryFlags.LMEM_FIXED;
            void* pointer = Kernel32LocalMemory.LocalAlloc(flags, (uint)(elementCount * elementSize));

            if(pointer == null)
            {
                throw new OutOfMemoryException();
            }

            return pointer;
        }

        public override unsafe void* Reallocate(void* pointer, int elementCount, int elementSize = 1, bool initMemory = true)
        {
            if (pointer == null)
            {
                throw new ArgumentException("Invalid pointer");
            }

            if (elementCount <= 0)
            {
                throw new ArgumentException(nameof(elementCount));
            }

            if (elementSize <= 0)
            {
                throw new ArgumentException(nameof(elementSize));
            }

            LocalMemoryFlags flags = initMemory ? LocalMemoryFlags.LMEM_ZEROINIT : LocalMemoryFlags.LMEM_MOVEABLE;
            Marshal.ReAllocHGlobal(default, default);
            void* newPointer = Kernel32LocalMemory.LocalAlloc(flags, (uint)elementCount);

            if (newPointer == null)
            {
                throw new OutOfMemoryException();
            }

            return newPointer;
        }

        public override unsafe void Free(void* pointer)
        {
            if (pointer == null)
            {
                throw new ArgumentException("Invalid pointer");
            }

            if (Kernel32LocalMemory.LocalFree(pointer) != null)
            {
                throw new ArgumentException("Invalid pointer");
            }
        }

        public int SizeOf(void* pointer)
        {
            return (int)Kernel32LocalMemory.LocalSize(pointer);
        }
    }
}