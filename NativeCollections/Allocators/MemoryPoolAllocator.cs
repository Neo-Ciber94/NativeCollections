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
    unsafe public sealed class MemoryPoolAllocator : Allocator, IDisposable
    {
        struct Chunk
        {
            public Chunk* next;
        }

        private const int DefaultBytesPerChunk = 1024;
        private const int DefaultChunkCount = 10;

        private readonly Allocator _defaultAllocator;

        private Chunk* _head;
        private byte* _bufferStart;
        private byte* _bufferEnd;
        private int _chunkCount;
        private int _chunkSize;

        /// <summary>
        /// Gets the number of chunks of this pool.
        /// </summary>
        /// <value>
        /// The number of chunks.
        /// </value>
        public int ChunkCount => _chunkCount;

        /// <summary>
        /// Gets the number of bytes per chunk.
        /// </summary>
        /// <value>
        /// The bytes per chunk.
        /// </value>
        public int BytesPerChunk => _chunkSize;

        /// <summary>
        /// Gets the bytes threshold for small allocations.
        /// </summary>
        /// <value>
        /// The bytes threshold.
        /// </value>
        public int BytesThreshold => _chunkSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPoolAllocator"/> class.
        /// </summary>
        /// <param name="bytesPerChunk">The bytes per chunk which also is the threshold for small allocations.</param>
        public MemoryPoolAllocator(int bytesPerChunk) : this(DefaultChunkCount, bytesPerChunk, Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPoolAllocator"/> class.
        /// </summary>
        /// <param name="chunkCount">The number of chunks in the pool.</param>
        /// <param name="bytesPerChunk">The bytes per chunk which also is the threshold for small allocations.</param>
        /// <param name="allocator">The allocator.</param>
        public MemoryPoolAllocator(int chunkCount, int bytesPerChunk) : this(chunkCount, bytesPerChunk, Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryPoolAllocator"/> class.
        /// </summary>
        /// <param name="chunkCount">The number of chunks in the pool.</param>
        /// <param name="bytesPerChunk">The bytes per chunk which also is the threshold for small allocations.</param>
        /// <param name="allocator">The allocator.</param>
        public MemoryPoolAllocator(int chunkCount, int bytesPerChunk, Allocator allocator) : base(true)
        {
            if (chunkCount <= 0)
            {
                throw new ArgumentException("chunkCount cannot be negative or 0", nameof(chunkCount));
            }

            if (bytesPerChunk <= 0)
            {
                throw new ArgumentException("bytesPerChunk cannot be negative or 0", nameof(bytesPerChunk));
            }

            int totalBytes = chunkCount * Math.Max(IntPtr.Size, bytesPerChunk);

            _defaultAllocator = allocator;
            _bufferStart = (byte*)Default.Allocate(totalBytes, initMemory: false);
            _bufferEnd = _bufferStart + (totalBytes);
            _chunkCount = chunkCount;
            _chunkSize = bytesPerChunk;

            for (int i = 0; i < chunkCount; ++i)
            {
                Chunk* cur = (Chunk*)(_bufferStart + (bytesPerChunk * i));
                cur->next = _head;
                _head = cur;
            }
        }

        public override unsafe void* Allocate(int elementCount, int elementSize = 1, bool initMemory = true)
        {
            if (_bufferStart == null)
            {
                throw new InvalidOperationException("MemoryPool have been disposed");
            }

            if (elementCount <= 0)
            {
                throw new ArgumentException(elementCount.ToString(), nameof(elementCount));
            }

            if (elementSize <= 0)
            {
                throw new ArgumentException(elementSize.ToString(), nameof(elementSize));
            }

            int totalBytes = elementCount * elementSize;

            if(totalBytes <= _chunkSize)
            {
                Chunk* block = _head;
                _head = _head->next;
                block->next = null;

                if (initMemory)
                {
                    Unsafe.InitBlockUnaligned(block, 0, (uint)_chunkSize);
                }

                return block;
            }

            return _defaultAllocator.Allocate(totalBytes, sizeof(byte), initMemory);
        }

        public override unsafe void* Reallocate(void* pointer, int elementCount, int elementSize = 1, bool initMemory = true)
        {
            if (_bufferStart == null)
            {
                throw new InvalidOperationException("MemoryPool have been disposed");
            }

            if (elementCount <= 0)
            {
                throw new ArgumentException(elementCount.ToString(), nameof(elementCount));
            }

            if (elementSize <= 0)
            {
                throw new ArgumentException(elementSize.ToString(), nameof(elementSize));
            }

            int totalBytes = elementCount * elementSize;

            if (IsOwner(pointer))
            {
                if(totalBytes > BytesThreshold)
                {
                    void* block = _defaultAllocator.Allocate(totalBytes, sizeof(byte), initMemory);
                    Unsafe.CopyBlockUnaligned(block, pointer, (uint)BytesThreshold);
                    Free(pointer);
                    return block;
                }

                return pointer;
            }

            return _defaultAllocator.Reallocate(pointer, totalBytes, sizeof(byte), initMemory);
        }

        public override unsafe void Free(void* pointer)
        {
            if (_bufferStart == null)
            {
                throw new InvalidOperationException("MemoryPool have been disposed");
            }

            if (IsOwner(pointer))
            {
                Chunk* chunk = (Chunk*)pointer;
                chunk->next = _head;
                _head = chunk;
            }
            else
            {
                _defaultAllocator.Free(pointer);
            }
        }

        public void Dispose()
        {
            if(_defaultAllocator is IDisposable disposable)
            {
                disposable.Dispose();
            }

            if (_bufferStart != null)
            {
                Default.Free(_bufferStart);
                _bufferStart = null;
                _bufferEnd = null;
                _head = null;
                _chunkCount = 0;
                _chunkSize = 0;
                Dispose(true);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsOwner(void* block)
        {
            return block != null && block >= _bufferStart && block < _bufferEnd;
        }
    }
}
