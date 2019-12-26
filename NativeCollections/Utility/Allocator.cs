using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NativeCollections.Utility
{
    unsafe public static class Allocator
    {
        public static T* Alloc<T>(int elementCount) where T: unmanaged
        {
            int totalBytes = sizeof(T) * elementCount;
            return (T*)Alloc(totalBytes);
        }

        public static void* Alloc(int size, bool zeroMemory = true)
        {
            if (size <= 0)
                throw new ArgumentException($"size must be greater than 0: {size}");

            void* pointer = (void*)Marshal.AllocHGlobal(size);
            if (zeroMemory)
            {
                ref byte startPos = ref *(byte*)pointer;
                Unsafe.InitBlockUnaligned(ref startPos, 0, (uint)size);
            }
            return pointer;
        }

        public static void* ReAlloc(void* pointer, int size)
        {
            if (size <= 0)
                throw new ArgumentException($"size must be greater than 0: {size}");

            if (pointer == null)
                throw new ArgumentException("Invalid pointer");

            var ptr = new IntPtr(pointer);
            var newsize = new IntPtr(size);
            return (void*)Marshal.ReAllocHGlobal(ptr, newsize);
        }

        public static void Free(void* pointer)
        {
            if (pointer == null)
                throw new ArgumentException("Invalid pointer");

            Marshal.FreeHGlobal((IntPtr)pointer);
        }
    }
}
