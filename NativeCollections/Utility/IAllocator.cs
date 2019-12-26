using System;
using System.Collections.Generic;
using System.Text;

namespace NativeCollections.Utility
{
    unsafe public interface IAllocator
    {
        public void* Allocate(int totalBytes);

        public void* ReAllocate(void* pointer, int totalBytes);

        public void Free(void* pointer);

        public int SizeOf(void* pointer);

        public int MaxSize { get; }
    }
}
