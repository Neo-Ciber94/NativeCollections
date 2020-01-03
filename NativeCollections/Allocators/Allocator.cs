using System;
using System.Runtime.CompilerServices;
using NativeCollections.Allocators.Internal;

namespace NativeCollections.Allocators
{
    /// <summary>
    /// Represents an operation performed over the elements of a <see cref="Span{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the span elements.</typeparam>
    /// <param name="span">The span.</param>
    public delegate void SpanAction<T>(Span<T> span);

    public abstract class Allocator
    {
        private const int MaxAllocatorCacheSize = 12;
        private static readonly Allocator?[] _cacheAllocators = new Allocator[MaxAllocatorCacheSize];
        private static int _nextID = 1;

        unsafe public static Allocator Default { get; }

        static Allocator()
        {
            //Default = DefaultHeapAllocator.Instance;
#if DEBUG
            unsafe
            {
                DefaultHeapAllocator defaultAllocator = DefaultHeapAllocator.Instance;
                Default = new DebugAllocator(defaultAllocator, defaultAllocator.SizeOf);
            }
#else
            Default = DefaultHeapAllocator.Instance;
#endif
        }

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

        /// <summary>
        /// Allocates the specified amount of elements and perform an operation over them using a <see cref="Span{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the allocated elements.</typeparam>
        /// <param name="elementCount">The number of elements to allocate.</param>
        /// <param name="action">The action to perform over the elements using a span.</param>
        public unsafe void Borrow<T>(int elementCount, SpanAction<T> action) where T: unmanaged
        {
            void* memBlock = Allocate<T>(elementCount);

            try
            {
                action(new Span<T>(memBlock, elementCount));
            }
            finally
            {
                Free(memBlock);
            }
        }

        /// <summary>
        /// Allocates the specified amount of elements and perform an operation over them using a <see cref="Span{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the allocated elements.</typeparam>
        /// <param name="elementCount">The number of elements to allocate.</param>
        /// <param name="stackAlloc">If <c>true</c> and the amount of elements is small the allocation may occur in the stack.</param>
        /// <param name="action">The action to perform over the elements using a span.</param>
        public unsafe void Borrow<T>(int elementCount, bool stackAlloc, SpanAction<T> action) where T : unmanaged
        {
#if X64
            const int bytesThreshold = 500000;
#else
            const int bytesThreshold = 250000;
#endif
            int totalBytes = sizeof(T) * elementCount;
            
            if(totalBytes > bytesThreshold && stackAlloc)
            {
                void* memBlock = stackalloc byte[totalBytes];
                action(new Span<T>(memBlock, elementCount));
            }
            else
            {
                void* memBlock = Allocate(totalBytes);

                try
                {
                    action(new Span<T>(memBlock, elementCount));
                }
                finally
                {
                    Free(memBlock);
                }
            }
        }
    }
}
