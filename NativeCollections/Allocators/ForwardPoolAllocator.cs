using System;
using System.Runtime.CompilerServices;

namespace NativeCollections.Allocators
{
    /// <summary>
    /// Represents a pool allocator that allocates memory with an external allocator if the
    /// bytes required are greater than certain threshold defined by the bytes per pool chunk.
    /// </summary>
    /// <seealso cref="Allocator" />
    /// <seealso cref="IDisposable" />
    public sealed class ForwardPoolAllocator : Allocator, IDisposable
    {
        private FixedMemoryPoolAllocator _poolAllocator;
        private Allocator _defaultAllocator;

        private const int DefaultChunkCount = 10;

        /// <summary>
        /// Gets the bytes threshold for small allocations.
        /// </summary>
        /// <value>
        /// The bytes threshold.
        /// </value>
        public int BytesThreshold
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _poolAllocator.BytesPerChunk;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardPoolAllocator"/> class.
        /// </summary>
        /// <param name="bytesPerChunk">The bytes per chunk which also is the threshold for small allocations.</param>
        public ForwardPoolAllocator(int bytesPerChunk) : this(DefaultChunkCount, bytesPerChunk, Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardPoolAllocator"/> class.
        /// </summary>
        /// <param name="chunkCount">The number of chunks in the pool.</param>
        /// <param name="bytesPerChunk">The bytes per chunk which also is the threshold for small allocations.</param>
        /// <param name="allocator">The allocator.</param>
        public ForwardPoolAllocator(int chunkCount, int bytesPerChunk, Allocator allocator) : base(true)
        {
            _poolAllocator = new FixedMemoryPoolAllocator(chunkCount, bytesPerChunk);
            _defaultAllocator = allocator;
        }

        public override unsafe void* Allocate(int elementCount, int elementSize = 1, bool initMemory = true)
        {
            int totalBytes = elementCount * elementSize;

            if(totalBytes <= BytesThreshold)
            {
                return _poolAllocator.Allocate(totalBytes, sizeof(byte), initMemory);
            }

            return _defaultAllocator.Allocate(totalBytes, sizeof(byte), initMemory);
        }

        public override unsafe void* Reallocate(void* pointer, int elementCount, int elementSize = 1, bool initMemory = true)
        {
            int totalBytes = elementCount * elementSize;

            if (_poolAllocator.IsOwner(pointer))
            {
                if(totalBytes > BytesThreshold)
                {
                    void* block = _defaultAllocator.Allocate(totalBytes, sizeof(byte), initMemory);
                    Unsafe.CopyBlockUnaligned(block, pointer, (uint)BytesThreshold);
                    _poolAllocator.Free(pointer);
                    return block;
                }

                return _poolAllocator.Reallocate(pointer, totalBytes, sizeof(byte), initMemory);
            }

            return _defaultAllocator.Reallocate(pointer, totalBytes, sizeof(byte), initMemory);
        }

        public override unsafe void Free(void* pointer)
        {
            if (_poolAllocator.IsOwner(pointer))
            {
                _poolAllocator.Free(pointer);
                return;
            }

            _defaultAllocator.Free(pointer);
        }

        public void Dispose()
        {
            if(_defaultAllocator is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _poolAllocator.Dispose();
            Dispose(true);
        }
    }
}
