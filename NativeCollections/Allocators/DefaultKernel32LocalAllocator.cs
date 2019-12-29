using System;
using System.Runtime.InteropServices;
using NativeCollections.Internal;

namespace NativeCollections.Allocators
{
    unsafe internal sealed class DefaultKernel32LocalAllocator : Allocator
    {
        public static readonly DefaultKernel32LocalAllocator Instance = new DefaultKernel32LocalAllocator();

        private DefaultKernel32LocalAllocator() { }

        public override unsafe void* Allocate(int size, int alignment, bool initMemory = true)
        {
            if (size <= 0)
            {
                throw new ArgumentException(nameof(size));
            }

            if(alignment <= 0)
            {
                throw new ArgumentException(nameof(alignment));
            }

            LocalMemoryFlags flags = initMemory? LocalMemoryFlags.LPTR: LocalMemoryFlags.LMEM_FIXED;
            void* pointer = Kernel32LocalMemory.LocalAlloc(flags, (uint)(size * alignment));

            if(pointer == null)
            {
                throw new OutOfMemoryException();
            }

            return pointer;
        }

        public override unsafe void* Reallocate(void* pointer, int size, int alignment = 1, bool initMemory = true)
        {
            if (pointer == null)
            {
                throw new ArgumentException("Invalid pointer");
            }

            if (size <= 0)
            {
                throw new ArgumentException(nameof(size));
            }

            if (alignment <= 0)
            {
                throw new ArgumentException(nameof(alignment));
            }

            LocalMemoryFlags flags = initMemory ? LocalMemoryFlags.LMEM_ZEROINIT : LocalMemoryFlags.LMEM_MOVEABLE;
            Marshal.ReAllocHGlobal(default, default);
            void* newPointer = Kernel32LocalMemory.LocalAlloc(flags, (uint)size);

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