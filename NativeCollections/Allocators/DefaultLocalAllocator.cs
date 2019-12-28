using System;
using NativeCollections.Internal;

namespace NativeCollections.Allocators
{
    unsafe internal sealed class DefaultLocalAllocator : Allocator
    {
        public static readonly DefaultLocalAllocator Instance = new DefaultLocalAllocator();

        private DefaultLocalAllocator() { }

        public override unsafe void* Allocate(int bytes, bool initMemory = true)
        {
            if (bytes <= 0)
            {
                throw new ArgumentException(nameof(bytes));
            }

            LocalMemoryFlags flags = initMemory? LocalMemoryFlags.LPTR: LocalMemoryFlags.LMEM_FIXED;
            void* pointer = LocalMemory.LocalAlloc(flags, (uint)bytes);

            if(pointer == null)
            {
                throw new OutOfMemoryException();
            }

            return pointer;
        }

        public override unsafe void Free(void* pointer)
        {
            if(pointer == null)
            {
                throw new ArgumentException("Invalid pointer");
            }

            if(LocalMemory.LocalFree(pointer) != null)
            {
                throw new ArgumentException("Invalid pointer");
            }
        }

        public override unsafe void* ReAllocate(void* pointer, int bytes)
        {
            if (pointer == null)
            {
                throw new ArgumentException("Invalid pointer");
            }

            if (bytes <= 0)
            {
                throw new ArgumentException(nameof(bytes));
            }

            void* newPointer = LocalMemory.LocalAlloc(LocalMemoryFlags.LMEM_ZEROINIT, (uint)bytes);

            if (newPointer == null)
            {
                throw new OutOfMemoryException();
            }

            return newPointer;
        }
    
        public int SizeOf(void* pointer)
        {
            return (int)LocalMemory.LocalSize(pointer);
        }
    }
}