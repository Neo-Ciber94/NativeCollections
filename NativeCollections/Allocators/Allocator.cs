using System;

namespace NativeCollections.Allocators
{
    public delegate void SpanAction<T>(Span<T> span);

    unsafe public abstract class Allocator
    {
        private static Allocator? _defaultAllocator;

        public static Allocator Default
        {
            get
            {
                if(_defaultAllocator == null)
                {
                    _defaultAllocator = DefaultHeapAllocator.Instance;
                }

                return _defaultAllocator;
            }
        }

        public abstract void* Allocate(int bytes, bool initMemory = true);

        public abstract void* ReAllocate(void* pointer, int bytes);

        public abstract void Free(void* pointer);

        public T* Allocate<T>(int elementCount) where T: unmanaged
        {
            return (T*)Allocate(sizeof(T) * elementCount);
        }

        public T* ReAllocate<T>(void* pointer, int elementCount) where T: unmanaged
        {
            return (T*)ReAllocate(pointer, sizeof(T) * elementCount);
        }

        public void Borrow<T>(int elementCount, SpanAction<T> action) where T : unmanaged
        {
            T* pointer = Allocate<T>(elementCount);

            try
            {
                action(new Span<T>(pointer, elementCount));
            }
            finally
            {
                Free(pointer);
            }
        }
    }
}