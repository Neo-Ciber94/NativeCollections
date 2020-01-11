using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using NativeCollections.Allocators;

namespace NativeCollections.Buffers
{
    /// <summary>
    /// Represents a fixed-size contiguous region of memory and provides methods for write and read to it.
    /// </summary>
    /// <seealso cref="INativeContainer{byte}" />
    /// <seealso cref="IDisposable" />
    [DebuggerDisplay("Length = {Length}")]
    [DebuggerTypeProxy(typeof(NativeBufferDebugView))]
    unsafe public struct NativeBuffer : INativeContainer<byte>, IDisposable
    {
        internal byte* _buffer;
        private int _length;
        private int _offset;
        private int _allocatorID;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeBuffer"/> struct.
        /// </summary>
        /// <param name="bytesCount">The number of bytes to allocate.</param>
        public NativeBuffer(int bytesCount) : this(bytesCount, Allocator.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeBuffer"/> struct.
        /// </summary>
        /// <param name="bytesCount">The number of bytes to allocate.</param>
        /// <param name="allocator">The allocator.</param>
        /// <exception cref="ArgumentException">If bytes count is negative or zero, or the allocator is not in cache.</exception>
        public NativeBuffer(int bytesCount, Allocator allocator)
        {
            if(bytesCount <= 0)
            {
                throw new ArgumentException("bytesCount cannot be negative or zero");
            }

            if(Allocator.IsCached(allocator) is false)
            {
                throw new ArgumentException("The allocator is not in cache");
            }

            _buffer = (byte*)allocator.Allocate(bytesCount);
            _length = bytesCount;
            _offset = 0;
            _allocatorID = allocator.ID;
        }

        /// <summary>
        /// Gets the number of bytes allocated.
        /// </summary>
        /// <value>
        /// The bytes allocated.
        /// </value>
        public int Capacity => _length;

        /// <summary>
        /// Gets the number of bytes written.
        /// </summary>
        /// <value>
        /// The number of bytes written.
        /// </value>
        public int Length => _offset;

        /// <summary>
        /// Checks if this instance is allocated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid => _buffer != null;

        /// <summary>
        /// Gets a value indicating whether this instance have bytes written..
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance don't have bytes written; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => _offset == 0;

        /// <summary>
        /// Gets the <see cref="byte"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="byte"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">If the index is out of range.</exception>
        public ref byte this[int index]
        {
            get
            {
                if(_buffer == null)
                {
                    throw new InvalidOperationException("NativeBuffer is invalid");
                }

                if (index < 0 || index > _offset)
                    throw new ArgumentOutOfRangeException(nameof(index), index.ToString());

                return ref _buffer[index];
            }
        }

        /// <summary>
        /// Gets the allocator.
        /// </summary>
        /// <returns></returns>
        public Allocator? GetAllocator()
        {
            return Allocator.GetAllocatorByID(_allocatorID);
        }

        /// <summary>
        /// Writes the specified value into the buffer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <exception cref="InvalidOperationException">If the buffer don't have memory to write to.</exception>
        public void Write<T>(T value) where T: unmanaged
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeBuffer is invalid");
            }

            if (!TryWrite(value))
            {
                throw new InvalidOperationException("Buffer don't have memory left to write");
            }
        }

        /// <summary>
        /// Attemps to write the value to the buffer if there is free memory.
        /// </summary>
        /// <typeparam name="T">Type of the value to write.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the write succeed.</returns>
        public bool TryWrite<T>(T value) where T : unmanaged
        {
            int sizeOfT = sizeof(T);
            byte* end = _buffer + _length;
            byte* next = _buffer + _offset + sizeOfT;

            if(next <= end)
            {
                T* cur = (T*)(_buffer + _offset);
                *cur = value;
                _offset += sizeOfT;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads the value at the specified byte offset.
        /// </summary>
        /// <typeparam name="T">Type of the value to read.</typeparam>
        /// <param name="byteOffset">The byte offset.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Byte offset is out of range</exception>
        public T Read<T>(int byteOffset) where T: unmanaged
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeBuffer is invalid");
            }

            if (!TryRead(byteOffset, out T value))
            {
                throw new InvalidOperationException("Byte offset is out of range");
            }

            return value;
        }

        /// <summary>
        /// Attemps to read the value at the specified offset.
        /// </summary>
        /// <typeparam name="T">Type of the value to read.</typeparam>
        /// <param name="byteOffset">The byte offset.</param>
        /// <param name="value">The readen value.</param>
        /// <returns><c>true</c> if the the offset is in the bounds and the value was readen.</returns>
        public bool TryRead<T>(int byteOffset, out T value) where T: unmanaged
        {
            if(byteOffset < 0)
            {
                value = default;
                return false;
            }

            int sizeOfT = sizeof(T);
            if(byteOffset + sizeOfT <= _length)
            {
                byte* ptr = _buffer + byteOffset;
                value = *(T*)ptr;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Attemps to read the values at the given range.
        /// </summary>
        /// <param name="byteOffset">The byte offset.</param>
        /// <param name="bytesCount">The number of bytes to read.</param>
        /// <param name="destination">The destination.</param>
        /// <returns><c>true</c> if the byteOffset and byteCount are into the bounds and the bytes were readen, otherwise <c>false</c>.</returns>
        public bool TryRead(int byteOffset, int bytesCount, in Span<byte> destination)
        {
            if (byteOffset < 0 || byteOffset > _offset)
                return false;

            if (bytesCount < 0 || bytesCount > (_offset - byteOffset) || bytesCount > destination.Length)
                return false;

            byte* dst = (byte*)Unsafe.AsPointer(ref destination.GetPinnableReference());
            Unsafe.CopyBlockUnaligned(dst, _buffer + byteOffset, (uint)bytesCount);
            return true;
        }

        /// <summary>
        /// Clears the written bytes.
        /// </summary>
        public void Clear()
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeBuffer is invalid");
            }

            if (_offset == 0)
                return;

            Unsafe.InitBlockUnaligned(_buffer, 0, (uint)_length);
            _offset = 0;
        }

        /// <summary>
        /// Copies the bytes of this buffer to a <see cref="Span{T}" />.
        /// </summary>
        /// <param name="span">The destination span to copy the data.</param>
        /// <param name="index">Start index of the destination where start to copy.</param>
        /// <param name="count">The number of bytes to copy.</param>
        /// <exception cref="ArgumentException">Span is empty</exception>
        /// <exception cref="InvalidOperationException">NativeBuffer is invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the index or count are out of range.</exception>
        public void CopyTo(in Span<byte> span, int index, int count)
        {
            if (span.IsEmpty)
                throw new ArgumentException("Span is empty");

            if (_buffer == null)
                throw new InvalidOperationException("NativeBuffer is invalid");

            if (index < 0 || index > span.Length)
                throw new ArgumentOutOfRangeException(nameof(index), index.ToString());

            if (count < 0 || count > _offset || count > (span.Length - index))
                throw new ArgumentOutOfRangeException(nameof(count), count.ToString());

            byte* dst = (byte*)Unsafe.AsPointer(ref span.GetPinnableReference());
            Unsafe.CopyBlockUnaligned(dst, _buffer, (uint)_offset);
        }

        /// <summary>
        /// Gets a pointer to this buffer data.
        /// </summary>
        /// <returns>A pointer to the first byte of this buffer.</returns>
        public byte* GetUnsafePointer()
        {
            return _buffer;
        }

        /// <summary>
        /// Allocates a new array with the bytes of this instance.
        /// </summary>
        /// <returns>A byte array with the bytes of this instance.</returns>
        public byte[] ToArray()
        {
            if (_buffer == null)
            {
                throw new InvalidOperationException("NativeBuffer is invalid");
            }

            if (_offset == 0)
            {
                return Array.Empty<byte>();
            }

            byte[] array = new byte[_offset];
            CopyTo(array, 0, _offset);
            return array;
        }

        /// <summary>
        /// Gets an enumerator over the bytes of this instance.
        /// </summary>
        /// <returns>An enumerator over the bytes of this instance.</returns>
        public RefEnumerator<byte> GetEnumerator()
        {
            return _buffer == null ? default : new RefEnumerator<byte>(_buffer, _offset);
        }

        /// <summary>
        /// Releases the resources used for this instance.
        /// </summary>
        public void Dispose()
        {
            if (_buffer is null)
                return;

            Allocator? allocator = GetAllocator();
            if(allocator != null)
            {
                allocator.Free(_buffer);
                this = default;
            }
        }
    }
}
