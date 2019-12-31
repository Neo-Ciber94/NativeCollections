using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NativeCollections.Allocators
{
    unsafe public sealed class ArenaAllocator : Allocator, IDisposable
    {
        private byte* _buffer;
        private int _length;
        private byte* _prevOffset;
        private byte* _offset;

        public ArenaAllocator(int totalBytes) : base(true)
        {
            if (totalBytes <= 0)
                throw new ArgumentException(nameof(totalBytes));

            _buffer = (byte*)Default.Allocate(totalBytes);
            _length = totalBytes;
            _offset = _buffer;
        }

        public override unsafe void* Allocate(int elementCount, int elementSize = 1, bool initMemory = true)
        {
            if (elementCount <= 0)
            {
                throw new ArgumentException(elementCount.ToString(), nameof(elementCount));
            }

            if (elementSize <= 0)
            {
                throw new ArgumentException(elementSize.ToString(), nameof(elementSize));
            }

            if (_buffer == null)
            {
                throw new InvalidOperationException("ArenaAllocator have been disposed");
            }

            int totalBytes = elementCount * elementSize;
            byte* next = _offset + totalBytes;
            byte* end = _buffer + _length;

            if(next > end)
            {
                throw new OutOfMemoryException();
            }

            byte* ptr = _offset;
            _prevOffset = _offset;
            _offset = next;
            return ptr;
        }

        public override void* Reallocate(void* pointer, int elementCount, int elementSize = 1, bool initMemory = true)
        {
            if (pointer == null)
                throw new ArgumentException("The pointer is null");

            if (elementCount <= 0)
                throw new ArgumentException(elementCount.ToString(), nameof(elementCount));

            if (elementSize <= 0)
                throw new ArgumentException(elementSize.ToString(), nameof(elementSize));

            if (!IsOwner(pointer))
                throw new InvalidOperationException("ArenaAllocator don't owns the given memory block.");

            if (_prevOffset == pointer)
            {
                int prevBlockSize = (int)(_offset - _prevOffset);
                int totalBytes = (elementCount * elementSize) - prevBlockSize;

                if(totalBytes <= 0)
                {
                    return pointer;
                }
                else
                {
                    byte* next = _offset + totalBytes;
                    byte* end = _buffer + _length;

                    if (next > end)
                    {
                        throw new OutOfMemoryException();
                    }

                    _offset += totalBytes;
                    return _prevOffset;
                }
            }
            else
            {
                return Allocate(elementCount, elementSize, initMemory);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override unsafe void Free(void* pointer) 
        {
            // There is not memory free in arena allocators
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsOwner(void* pointer) => _buffer != null && pointer >= _buffer && pointer < (_buffer + _length);

        public void Dispose()
        {
            if (_buffer == null)
                return;

            Dispose(true);
            Default.Free(_buffer);
            GC.SuppressFinalize(this);

            _buffer = null;
            _length = 0;
            _offset = null;
            _prevOffset = null;
        }

        ~ArenaAllocator()
        {
            Dispose(false);
        }
    }
}
