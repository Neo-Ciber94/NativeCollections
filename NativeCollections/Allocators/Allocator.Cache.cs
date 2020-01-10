using System.Runtime.CompilerServices;
using NativeCollections.Allocators.Internal;

namespace NativeCollections.Allocators
{
    public abstract partial class Allocator
    {
        private const int MaxAllocatorCacheSize = 12;
        private static readonly Allocator?[] _cacheAllocators = new Allocator[MaxAllocatorCacheSize];
        private static int _nextID = 0;

        /// <summary>
        /// Gets the default allocator.
        /// </summary>
        /// <value>
        /// The default allocator.
        /// </value>
        unsafe public static Allocator Default { get; } = DefaultHeapAllocator.Instance;

        static Allocator()
        {
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

        /// <summary>
        /// Adds the allocator to cache.
        /// </summary>
        /// <param name="allocator">The allocator to add</param>
        /// <returns>The id of the allocator added.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int AddToCache(Allocator allocator)
        {
            if (_nextID < MaxAllocatorCacheSize)
            {
                _cacheAllocators[_nextID] = allocator;
                return ++_nextID;
            }

            return -1;
        }

        /// <summary>
        /// Removes the allocator from cache.
        /// </summary>
        /// <param name="allocator">The allocator to remove.</param>
        /// <returns><c>true</c> if the allocator is in cache and was removed; otherwise <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool RemoveFromCache(Allocator allocator)
        {
            if (allocator != null)
            {
                int id = allocator.ID;
                if (id > 0 && id < MaxAllocatorCacheSize)
                {
                    _cacheAllocators[id - 1] = null;
                    --_nextID;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the allocator by its id.
        /// </summary>
        /// <param name="id">The id of the allocator.</param>
        /// <returns>The allocator with the specified id or null if not found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Allocator? GetAllocatorByID(int id)
        {
            if (id > 0 && id < MaxAllocatorCacheSize)
            {
                return _cacheAllocators[id - 1];
            }

            return null;
        }

        /// <summary>
        /// Determines whether exists an allocator in cached with the specified id.
        /// </summary>
        /// <param name="id">The id of the allocator.</param>
        /// <returns>
        ///   <c>true</c> if the allocator with the given id is in cache; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCached(int id)
        {
            if(id > 0 && id < MaxAllocatorCacheSize)
            {
                Allocator? allocator = _cacheAllocators[id - 1];
                if(allocator != null)
                {
                    return allocator.ID == id;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified allocator is cached.
        /// </summary>
        /// <param name="allocator">The allocator.</param>
        /// <returns>
        ///   <c>true</c> if the specified allocator is in cache; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCached(Allocator allocator)
        {
            int id = allocator.ID;
            if (id > 0 && id < MaxAllocatorCacheSize)
            {
                return _cacheAllocators[id - 1] == allocator;
            }

            return false;
        }
    }
}
