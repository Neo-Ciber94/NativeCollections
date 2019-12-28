using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NativeCollections.Allocators;
using NativeCollections.Utility;

namespace NativeCollections
{
    unsafe public struct NativeStack<T> : IDisposable where T : unmanaged
    {
        internal void* _buffer;
        private int _capacity;
        private int _count;

        public NativeStack(int initialCapacity)
        {
            if (initialCapacity <= 0)
                throw new ArgumentException($"capacity must be greater than 0: {initialCapacity}");

            _buffer = Allocator.Default.Allocate(sizeof(T) * initialCapacity);
            _capacity = initialCapacity;
            _count = 0;
        }

        public NativeStack(Span<int> elements)
        {
            if (elements.IsEmpty)
            {
                this = default;
            }
            else
            {
                _buffer = Allocator.Default.Allocate(sizeof(T) * elements.Length);
                _capacity = elements.Length;
                _count = _capacity;

                void* source = Unsafe.AsPointer(ref MemoryMarshal.GetReference(elements));
                Unsafe.CopyBlock(_buffer, source, (uint)(sizeof(T) * _capacity));
            }
        }

        public int Length => _count;

        public int Capacity => _capacity;

        public bool IsValid => _buffer != null;

        public bool IsEmpty => _count == 0;

        public void Push(T value)
        {
            if (_count == _capacity)
            {
                EnsureCapacity(_count + 1);
            }

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            Unsafe.Add(ref startAddress, _count++) = value;
        }

        public T Pop()
        {
            if (_count == 0)
                throw new InvalidOperationException("Stack is empty");

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            ref T value = ref Unsafe.Add(ref startAddress, _count);
            Unsafe.Add(ref startAddress, _count--) = default;
            return value;
        }

        public T Peek()
        {
            if (_count == 0)
                throw new InvalidOperationException("Stack is empty");

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            ref T value = ref Unsafe.Add(ref startAddress, _count);
            return value;
        }

        public bool TryPop(out T value)
        {
            if (_count == 0)
            {
                value = default;
                return false;
            }
            else
            {
                ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
                value = Unsafe.Add(ref startAddress, _count);
                Unsafe.Add(ref startAddress, _count--) = default;
                return true;
            }
        }

        public bool TryPeek(out T value)
        {
            if (_count == 0)
            {
                value = default;
                return false;
            }
            else
            {
                ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
                value = Unsafe.Add(ref startAddress, _count);
                return true;
            }
        }

        public void Clear()
        {
            if (_count == 0)
                return;

            ref byte pointer = ref Unsafe.As<T, byte>(ref Unsafe.AsRef<T>(_buffer));
            uint length = (uint)(sizeof(T) * _count);
            Unsafe.InitBlockUnaligned(ref pointer, 0, length);
            _count = 0;
        }

        public bool Contains(T value)
        {
            return NativeCollectionUtility.IndexOf<T>(_buffer, _capacity, value) >= 0;
        }

        public void TrimExcess()
        {
            if (_count != _capacity && _capacity > 4)
            {
                SetCapacity(_count);
            }
        }

        private void EnsureCapacity(int min)
        {
            if (min > _capacity)
            {
                if (min < _capacity * 2)
                {
                    SetCapacity(_capacity * 2);
                }
                else
                {
                    SetCapacity(min + 1);
                }
            }
        }

        private void SetCapacity(int newCapacity)
        {
            newCapacity = newCapacity < 4 ? 4 : newCapacity;
            Allocator.Default.ReAllocate(_buffer, sizeof(T) * newCapacity);
            _capacity = newCapacity;
        }

        public void Dispose()
        {
            if (_buffer == null)
                return;

            Allocator.Default.Free(_buffer);
            _buffer = null;
            _capacity = 0;
        }

        public RefEnumerator<T> GetEnumerator()
        {
            return new RefEnumerator<T>(_buffer, _capacity);
        }
    }
}
