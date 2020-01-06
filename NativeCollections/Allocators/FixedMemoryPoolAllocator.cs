using System;
using System.Runtime.CompilerServices;

namespace NativeCollections.Allocators
{
    /// <summary>
    /// A fixed-size memory pool allocator.
    /// </summary>
    /// <seealso cref="NativeCollections.Allocators.Allocator" />
    /// <seealso cref="System.IDisposable" />
    unsafe public sealed class FixedMemoryPoolAllocator : Allocator, IDisposable
    {
        struct Chunk
        {
            public Chunk* next;
        }

        private const int DefaultBytesPerChunk = 800;

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
        /// Initializes a new instance of the <see cref="FixedMemoryPoolAllocator"/> class.
        /// </summary>
        /// <param name="chunkCount">The number of chunks.</param>
        /// <exception cref="ArgumentException">If chunkCount is negative or 0.</exception>
        public FixedMemoryPoolAllocator(int chunkCount) : this(chunkCount, DefaultBytesPerChunk) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedMemoryPoolAllocator"/> class.
        /// </summary>
        /// <param name="chunkCount">The number of chunks.</param>
        /// <param name="bytesPerChunk">The size in bytes of each chunk.</param>
        /// <exception cref="ArgumentException">If chunkCount or bytesPerChunk are negative or 0.</exception>
        public FixedMemoryPoolAllocator(int chunkCount, int bytesPerChunk) : base(true)
        {
            if(chunkCount <= 0)
            {
                throw new ArgumentException("chunkCount cannot be negative or 0", nameof(chunkCount));
            }

            if(bytesPerChunk <= 0)
            {
                throw new ArgumentException("bytesPerChunk cannot be negative or 0", nameof(bytesPerChunk));
            }

            if(bytesPerChunk < IntPtr.Size)
            {
                bytesPerChunk = IntPtr.Size;
            }

            int totalBytes = chunkCount * bytesPerChunk;
            _bufferStart = (byte*)Default.Allocate(totalBytes);
            _bufferEnd = _bufferStart + (totalBytes);
            _chunkCount = chunkCount;
            _chunkSize = bytesPerChunk;

            for(int i = 0; i < chunkCount; ++i)
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

            if(_head == null)
            {
                throw new OutOfMemoryException("No chunks available");
            }

            if(totalBytes > _chunkSize)
            {
                throw new OutOfMemoryException($"Not enough memory for allocate {totalBytes} bytes.\nMax block size: {_chunkSize} bytes");
            }

            Chunk* block = _head;
            _head = _head->next;
            block->next = null;

            if (initMemory)
            {
                Unsafe.InitBlockUnaligned(block, 0, (uint)_chunkSize);
            }

            return block;
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

            if (!IsOwner(pointer))
            {
                throw new ArgumentException("Block not allocated in this MemoryPool");
            }

            int totalBytes = elementCount * elementSize;

            if (_head == null)
            {
                throw new OutOfMemoryException("No chunks available");
            }

            if (totalBytes > _chunkSize)
            {
                throw new OutOfMemoryException($"Not enough memory for allocate {totalBytes} bytes.\nMax block size: {_chunkSize} bytes");
            }

            return pointer;
        }

        public override unsafe void Free(void* pointer)
        {
            if (_bufferStart == null)
            {
                throw new InvalidOperationException("MemoryPool have been disposed");
            }

            if (!IsOwner(pointer))
            {
                throw new ArgumentException("Block not allocated in this MemoryPool");
            }

            Chunk* chunk = (Chunk*)pointer;
            chunk->next = _head;
            _head = chunk;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsOwner(void* block)
        {
            return block != null && block >= _bufferStart && block < _bufferEnd;
        }

        public void Dispose()
        {
            // Not all chunks may be returned at this point, what can lead to pointers to invalid memory.
            // That can be solved using a counter for each chunk and prevent the Dispose or throw an exception if any chunk
            // still allocated.

            if(_bufferStart != null)
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
    }
}
