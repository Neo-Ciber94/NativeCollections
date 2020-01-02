using System;
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
                Requires.IsTrue(BytesAllocated == 0, $"Memory leak detected, bytes allocted: {BytesAllocated}");
            };
        }

        public long BytesAllocated { get; private set; }

        public override unsafe void* Allocate(int elementCount, int elementSize = 1, bool initMemory = true)
        {
            Requires.IsTrue(elementCount > 0, "Invalid elementCount");
            Requires.IsTrue(elementSize > 0, "Invalid elementSize");

            BytesAllocated += (elementCount * elementSize);
            return _allocator.Allocate(elementCount, elementSize, initMemory);
        }

        public override unsafe void* Reallocate(void* pointer, int elementCount, int elementSize = 1, bool initMemory = true)
        {
            Requires.IsTrue(pointer != null, "Pointer is null");
            Requires.IsTrue(elementCount > 0, "Invalid elementCount");
            Requires.IsTrue(elementSize > 0, "Invalid elementSize");

            int size = _sizeOf(pointer);
            int newSize = elementCount * elementSize;
            int diff = size - newSize;

            BytesAllocated += diff;
            return _allocator.Reallocate(pointer, elementCount, elementSize, initMemory);
        }

        public override unsafe void Free(void* pointer)
        {
            Requires.IsTrue(pointer != null, "Pointer is null");

            int size = _sizeOf(pointer);
            _allocator.Free(pointer);
            BytesAllocated -= size;
        }

        public void Dispose()
        {
            Requires.IsTrue(BytesAllocated == 0, $"Memory leak detected, bytes allocted: {BytesAllocated}");

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
