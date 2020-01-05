using System;
using System.Diagnostics;
using NativeCollections.Utility;

namespace NativeCollections.Allocators
{
    unsafe public delegate int SizeOfDelegate(void* pointer);

    public sealed class DebugAllocator : Allocator, IDisposable
    {
        private readonly Allocator _allocator;
        private readonly SizeOfDelegate _sizeOf;

        public DebugAllocator(Allocator allocator, SizeOfDelegate sizeOfPointer) : base(true)
        {
#if RELEASE
            throw new NotSupportedException("DebugAllocator must not run on release mode.");
#endif
            _allocator = allocator;
            _sizeOf = sizeOfPointer;

            AppDomain.CurrentDomain.ProcessExit += (o, e) =>
            {
                Requires.Argument(BytesAllocated == 0, $"Memory leak detected, bytes allocted: {BytesAllocated}");
            };
        }

        public long BytesAllocated { get; private set; }

        public override unsafe void* Allocate(int elementCount, int elementSize = 1, bool initMemory = true)
        {
            Requires.Argument(elementCount > 0, "Invalid elementCount");
            Requires.Argument(elementSize > 0, "Invalid elementSize");

            BytesAllocated += (elementCount * elementSize);
            return _allocator.Allocate(elementCount, elementSize, initMemory);
        }

        public override unsafe void* Reallocate(void* pointer, int elementCount, int elementSize = 1, bool initMemory = true)
        {
            Requires.Argument(pointer != null, "Pointer is null");
            Requires.Argument(elementCount > 0, "Invalid elementCount");
            Requires.Argument(elementSize > 0, "Invalid elementSize");

            int size = _sizeOf(pointer);
            Requires.Argument(size > 0);

            int newSize = elementCount * elementSize;
            int diff = newSize - size;

            BytesAllocated += diff;
            return _allocator.Reallocate(pointer, elementCount, elementSize, initMemory);
        }

        public override unsafe void Free(void* pointer)
        {
            Requires.Argument(pointer != null, "Pointer is null");

            int size = _sizeOf(pointer);
            _allocator.Free(pointer);
            BytesAllocated -= size;
        }

        public void Dispose()
        {
            Requires.Argument(BytesAllocated == 0, $"Memory leak detected, bytes allocted: {BytesAllocated}");

            if(_allocator is IDisposable disposable)
            {
                disposable.Dispose();
                Dispose(true);
            }
        }

        ~DebugAllocator()
        {
            Dispose(false);
        }
    }
}
