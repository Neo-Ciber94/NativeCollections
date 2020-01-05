using System;
using System.Runtime.InteropServices;
using NativeCollections.Internal;

namespace NativeCollections.Allocators.Internal
{
    unsafe public sealed class DefaultLocalAllocator : Allocator
    {
        public static readonly DefaultLocalAllocator Instance = new DefaultLocalAllocator();

        private DefaultLocalAllocator() : base(true) { }

        public override unsafe void* Allocate(int elementCount, int elementSize = 1, bool initMemory = true)
        {
            if (elementCount <= 0)
            {
                throw new ArgumentException(elementCount.ToString(), nameof(elementCount));
            }

            if (elementSize <= 0)
            {
                throw new ArgumentException(elementSize.ToString(), nameof(elementSize));
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
                throw new ArgumentException(elementCount.ToString(), nameof(elementCount));
            }

            if (elementSize <= 0)
            {
                throw new ArgumentException(elementSize.ToString(), nameof(elementSize));
            }

            LocalMemoryFlags flags = initMemory ? LocalMemoryFlags.LMEM_MOVEABLE | LocalMemoryFlags.LMEM_ZEROINIT : LocalMemoryFlags.LMEM_MOVEABLE;
            void* newPointer = Kernel32LocalMemory.LocalReAlloc(pointer, (uint)(elementCount * elementSize), flags);

            if (newPointer == null)
            {
                Console.WriteLine(Marshal.GetLastWin32Error());
                throw new OutOfMemoryException();
            }

            return newPointer;
        }

        public override unsafe void Free(void* pointer)
        {

            if (Kernel32LocalMemory.LocalFree(pointer) != null)
            {
                throw new ArgumentException("Invalid pointer");
            }
        }

        public int SizeOf(void* pointer)
        {
            if (pointer == null)
            {
                throw new ArgumentException("Invalid pointer");
            }

            return (int)Kernel32LocalMemory.LocalSize(pointer);
        }
    }
}