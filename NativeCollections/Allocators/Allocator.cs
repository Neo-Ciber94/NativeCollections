using System;
using System.Runtime.CompilerServices;
using NativeCollections.Allocators.Internal;

namespace NativeCollections.Allocators
{
    public delegate void SpanAction<T>(Span<T> span);

    public abstract class Allocator
    {
        private const int MaxAllocatorCacheSize = 12;
        private static readonly Allocator?[] _cacheAllocators = new Allocator[MaxAllocatorCacheSize];
        private static int _nextID = 1;

        public static Allocator Default { get; } = DefaultKernel32HeapAllocator.Instance;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Allocator? GetAllocatorByID(int id)
        {
            if(id > 0 && id < MaxAllocatorCacheSize)
            {
                return _cacheAllocators[id - 1];
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCached(int id)
        {
            return id > 0 && id < MaxAllocatorCacheSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCached(Allocator allocator)
        {
            return IsCached(allocator.ID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int AddInstanceToCache(Allocator allocator) 
        {
            if(_nextID <= MaxAllocatorCacheSize)
            {
                _cacheAllocators[_nextID - 1] = allocator;
                return _nextID++;
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool RemoveInstanceFromCache(Allocator allocator)
        {
            if(allocator != null)
            {
                int id = allocator.ID;
                if(id > 0 && id < MaxAllocatorCacheSize)
                {
                    _cacheAllocators[id - 1] = null;
                    _nextID--;
                    return true;
                }
            }

            return false;
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

        #region Virtual and Abstract methods
        public abstract unsafe void* Allocate(int elementCount, int elementSize = 1, bool initMemory = true);

        public abstract unsafe void* Reallocate(void* pointer, int elementCount, int elementSize = 1, bool initMemory = true);
        
        public abstract unsafe void Free(void* pointer);

        protected virtual void Dispose(bool disposing)
        {
            if (IsCached(this))
            {
                RemoveInstanceFromCache(this);
            }
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe T* Allocate<T>(int elementCount) where T: unmanaged
        {
            return (T*)Allocate(elementCount, sizeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe T* Reallocate<T>(T* pointer, int elementCount) where T: unmanaged
        {
            return (T*)Reallocate(pointer, elementCount, sizeof(T));
        }

        [Obsolete("This method is experimental and could be removed")]
        public unsafe void Borrow<T>(int elementCount, SpanAction<T> action) where T: unmanaged
        {
            void* memory = Default.Allocate(elementCount, sizeof(T));

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
