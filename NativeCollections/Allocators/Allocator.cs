using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using NativeCollections.Allocators.Internal;

namespace NativeCollections.Allocators
{
    /// <summary>
    /// Represents a memory allocator.
    /// </summary>
    public abstract partial class Allocator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Allocator"/> class.
        /// </summary>
        protected Allocator() : this(false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Allocator"/> class.
        /// </summary>
        /// <param name="cacheInstance">if set to <c>true</c> this allocator will be added to cache.</param>
        protected Allocator(bool cacheInstance)
        {
            ID = cacheInstance ? AddToCache(this) : -1;
        }

        /// <summary>
        /// Gets the id of this allocator.
        /// </summary>
        /// <value>
        /// The id of the allocator.
        /// </value>
        public int ID { get; }

        /// <summary>
        /// Removes this instance from cache.
        /// </summary>
        /// <param name="disposing">Determines if this instance will be removed from cache.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                RemoveFromCache(this);
            }
        }

        #region Abstract methods        
        /// <summary>
        /// Allocates the specified amount of elements with the specified size.
        /// </summary>
        /// <param name="elementCount">The number of elements to allocate.</param>
        /// <param name="elementSize">Size of the elements.</param>
        /// <param name="initMemory">if set to <c>true</c> the memory will be initializated to 0.</param>
        /// <returns>A pointer to the allocated memory block.</returns>
        public abstract unsafe void* Allocate(int elementCount, int elementSize = 1, bool initMemory = true);

        /// <summary>
        /// Reallocates the specified memory block.
        /// </summary>
        /// <param name="pointer">A pointer to the allocated memory.</param>
        /// <param name="elementCount">The number of elements to allocate.</param>
        /// <param name="elementSize">Size of the elements.</param>
        /// <param name="initMemory">if set to <c>true</c> the memory will be initializated to 0.</param>
        /// <returns>A pointer to the allocated memory block.</returns>
        public abstract unsafe void* Reallocate(void* pointer, int elementCount, int elementSize = 1, bool initMemory = true);

        /// <summary>
        /// Releases the memory of the specified pointer to a memory block.
        /// </summary>
        /// <param name="pointer">The pointer to the memory block.</param>
        public abstract unsafe void Free(void* pointer);
        #endregion

        #region Default Methods        
        /// <summary>
        /// Allocates the specified amount of elements.
        /// </summary>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <param name="elementCount">The number of elements to allocate.</param>
        /// <returns>A pointer to the allocated memory block.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe T* Allocate<T>(int elementCount) where T: unmanaged
        {
            return (T*)Allocate(elementCount, sizeof(T));
        }

        /// <summary>
        /// Reallocates the memory to the specified memory block.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pointer">A pointer to the memory block to reallocate.</param>
        /// <param name="elementCount">The number of elements to allocate.</param>
        /// <returns>A pointer to the reallocated memory block.</returns>
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
        public unsafe void Borrow<T>(int elementCount, SpanAction<T, int> action) where T: unmanaged
        {
            void* memBlock = Allocate<T>(elementCount);

            try
            {
                action(new Span<T>(memBlock, elementCount), elementCount);
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
        public unsafe void Borrow<T>(int elementCount, bool stackAlloc, SpanAction<T, int> action) where T : unmanaged
        {
#if BIT64
            const int bytesThreshold = 500000;
#else
            const int bytesThreshold = 250000;
#endif
            int totalBytes = sizeof(T) * elementCount;
            
            if(totalBytes > bytesThreshold && stackAlloc)
            {
                void* memBlock = stackalloc byte[totalBytes];
                action(new Span<T>(memBlock, elementCount), elementCount);
            }
            else
            {
                void* memBlock = Allocate(totalBytes);

                try
                {
                    action(new Span<T>(memBlock, elementCount), elementCount);
                }
                finally
                {
                    Free(memBlock);
                }
            }
        }
        #endregion
    }
}
