using System;
using System.Runtime.CompilerServices;

namespace NativeCollections.Allocators
{
    public delegate void SpanAction<T>(Span<T> span);

    public abstract class Allocator
    {
        private const int MaxAllocatorCacheSize = 8;
        private static readonly Allocator[] _cacheAllocators = new Allocator[MaxAllocatorCacheSize];
        private static int _nextID = 0;

        public static Allocator Default { get; } = DefaultKernel32HeapAllocator.Instance;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Allocator? GetAllocator(int id)
        {
            if(id >= 0 && id < MaxAllocatorCacheSize)
            {
                return _cacheAllocators[id];
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int AddInstanceToCache(Allocator allocator) 
        {
            if(_nextID < MaxAllocatorCacheSize)
            {
                _cacheAllocators[_nextID] = allocator;
                return _nextID++;
            }

            return -1;
        }

        protected Allocator() { }
        
        protected Allocator(bool cacheInstance)
        {
            if (cacheInstance)
            {
                ID = AddInstanceToCache(this);
            }
        }

        public int ID { get; } = -1;

        unsafe public abstract void* Allocate(int size, int alignment = 1, bool initMemory = true);
        unsafe public abstract void* Reallocate(void* pointer, int size, int alignment = 1, bool initMemory = true);
        unsafe public abstract void Free(void* pointer);

        unsafe public T* Allocate<T>(int elementCount) where T : unmanaged => (T*)Allocate(elementCount, sizeof(T));

        unsafe public T* ReAllocate<T>(T* pointer, int elementCount) where T : unmanaged => (T*)Reallocate(pointer, elementCount, sizeof(T));

        [Obsolete("This method is experimental and could be removed")]
        unsafe public void Borrow<T>(int elementCount, SpanAction<T> action) where T: unmanaged
        {
            void* memory = Allocate<T>(elementCount);

            try
            {
                action(new Span<T>(memory, elementCount));
            }
            finally
            {
                Free(memory);
            }
        }
    }
}
