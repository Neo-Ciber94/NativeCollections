using System;
using System.Runtime.CompilerServices;

namespace NativeCollections.Allocators
{
    unsafe public sealed class StackAllocator : Allocator, IDisposable
    {
        private struct StackAllocatorHeader
        {
            public int size;
            public int offset;
        }

        private byte* _buffer;
        private int _length;
        private byte* _offset;
        private byte* _prevOffset;

        public StackAllocator(int totalBytes) : base(true)
        {
            if (totalBytes <= 0)
                throw new ArgumentException(totalBytes.ToString(), nameof(totalBytes));

            _buffer = (byte*)Default.Allocate(totalBytes); // + 4 * sizeof(StackAllocatorHeader)
            _length = totalBytes;
            _offset = _buffer;
        }

        public override unsafe void* Allocate(int elementCount, int elementSize = 1, bool initMemory = true)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("StackAllocator have been disposed");
            }

            if (elementCount <= 0)
            {
                throw new ArgumentException(elementCount.ToString(), nameof(elementCount));
            }

            if (elementSize <= 0)
            {
                throw new ArgumentException(elementSize.ToString(), nameof(elementSize));
            }

            int blockSize = elementCount * elementSize;
            int size = blockSize + sizeof(StackAllocatorHeader);
            byte* end = _buffer + _length;
            byte* nextOffset = _offset + size;

            if (nextOffset > end)
            {
                throw new OutOfMemoryException();
            }

            StackAllocatorHeader* header = (StackAllocatorHeader*)_offset;
            header->size = blockSize;
            header->offset = _prevOffset == null ? 0 : (int)(_prevOffset - _buffer);

            byte* block = _offset + sizeof(StackAllocatorHeader);

            if (initMemory)
            {
                Unsafe.InitBlockUnaligned(block, 0, (uint)blockSize);
            }

            _prevOffset = _offset;
            _offset += size;
            return block;
        }

        public override unsafe void* Reallocate(void* pointer, int elementCount, int elementSize = 1, bool initMemory = true)
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("StackAllocator have been disposed");
            }

            if (!IsOwner(pointer))
            {
                throw new ArgumentException("StackAllocator don't owns the given memory block.");
            }

            if (elementCount <= 0)
            {
                throw new ArgumentException(elementCount.ToString(), nameof(elementCount));
            }

            if (elementSize <= 0)
            {
                throw new ArgumentException(elementSize.ToString(), nameof(elementSize));
            }

            byte* block = (byte*)pointer;
            int blockSize = elementCount * elementSize;

            StackAllocatorHeader* header = (StackAllocatorHeader*)(block - sizeof(StackAllocatorHeader));

            if(header->size < blockSize)
            {
                return pointer;
            }

            if(header == _prevOffset)
            {
                int size = blockSize + sizeof(StackAllocatorHeader);
                byte* end = _buffer + _length;
                byte* nextOffset = _offset + size;

                if (nextOffset > end)
                {
                    throw new OutOfMemoryException();
                }

                int requiredSize = blockSize - header->size;

                if (initMemory)
                {
                    Unsafe.InitBlockUnaligned(_offset, 0, (uint)requiredSize);
                }

                header->size = size;
                _offset = nextOffset;

                return block;
            }
            else
            {
                throw new InvalidOperationException("Only the last allocated memory block can be reallocated.\n" +
                    $"Last memory block was {ToHex(_prevOffset)} but {ToHex(header)} was get.");
            }
        }

        public override unsafe void Free(void* pointer)
        {
            if (pointer == null)
            {
                throw new ArgumentException("pointer is null");
            }

            if (!IsOwner(pointer))
            {
                throw new ArgumentException("StackAllocator don't owns the given memory block.");
            }

            if (_buffer == null)
            {
                throw new InvalidOperationException("StackAllocator have been disposed");
            }

            byte* block = (byte*)pointer;
            StackAllocatorHeader* header = (StackAllocatorHeader*)(block - sizeof(StackAllocatorHeader));

            if (header != _prevOffset)
            {
                throw new InvalidOperationException($"Invalid memory block position.\n" +
                    $"Expected memory address: {ToHex(_prevOffset)} but {ToHex(header)} was get.\n" +
                    $"Any memory allocated within the StackAllocator must be free in a LIFO (Last-In First-Out) order.");
            }

            _offset -= header->size;
            _prevOffset -= header->offset;
        }

        public int SizeOf(void* block)
        {
            if (IsOwner(block))
            {
                var header = (StackAllocatorHeader*)((byte*)block - sizeof(StackAllocatorHeader));
                return header->size;
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsOwner(void* block) => _buffer != null && block >= _buffer && block < (_buffer + _length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string ToHex(void* pointer)
        {
            return "0x" + ((long)pointer).ToString("x16");
        }

        public void Dispose()
        {
            if (_buffer == null)
                return;

            Dispose(true);
            GC.SuppressFinalize(this);

            _buffer = null;
            _length = 0;
            _offset = null;
            _prevOffset = null;
        }

        ~StackAllocator()
        {
            Dispose(false);
        }
    }
}
