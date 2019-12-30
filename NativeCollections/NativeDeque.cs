using System;
using System.Collections.Generic;
using System.Text;
using NativeCollections.Allocators;

namespace NativeCollections
{
    unsafe public struct NativeDeque<T> : IDisposable where T : unmanaged
    {
        private T* _buffer;
        private int _capacity;
        private int _count;
        private int _allocatorID;

        private int _head;
        private int _tail;

        public NativeDeque(int initialCapacity) : this(initialCapacity, Allocator.Default) { }

        public NativeDeque(int initialCapacity, Allocator allocator)
        {
            if (initialCapacity <= 0)
                throw new ArgumentException("initialCapacity should be greater than 0.", nameof(initialCapacity));

            _buffer = allocator.Allocate<T>(initialCapacity);
            _capacity = initialCapacity;
            _allocatorID = allocator.ID;
            _count = 0;
            _head = 0;
            _tail = 0;
        }

        public int Length => _count;

        public int Capacity => _capacity;

        public bool IsEmpty => _count == 0;

        public bool IsValid => _buffer != null;

        public Allocator? GetAllocator()
        {
            return Allocator.GetAllocatorByID(_allocatorID);
        }

        public void AddFirst(T value)
        {
            if(_count == _capacity)
            {
                Resize();
            }

            _buffer[_head] = value;
            _head = (_head + 1) % _capacity;
            _count++;
        }

        public void AddLast(T value)
        {
            if (_count == _capacity)
            {
                Resize();
            }

            _tail = (_tail - 1) % _capacity;
            _buffer[_tail] = value;
            _count++;
        }

        public T PopFirst()
        {
            if(!TryPopFirst(out T value))
            {
                throw new InvalidOperationException("NativeDeque is empty");
            }

            return value;
        }

        public T PopLast()
        {
            if (!TryPopLast(out T value))
            {
                throw new InvalidOperationException("NativeDeque is empty");
            }

            return value;
        }

        public T PeekFirst()
        {
            if (!TryPeekFirst(out T value))
            {
                throw new InvalidOperationException("NativeDeque is empty");
            }

            return value;
        }

        public T PeekLast()
        {
            if (!TryPopLast(out T value))
            {
                throw new InvalidOperationException("NativeDeque is empty");
            }

            return value;
        }

        public bool TryPopFirst(out T value)
        {
            if(_count > 0)
            {
                value = _buffer[_head];
                _head = (_head - 1) % _capacity;
                _count--;
                return true;
            }

            value = default;
            return false;
        }

        public bool TryPopLast(out T value)
        {
            if (_count > 0)
            {
                value = _buffer[_tail];
                _head = (_tail + 1) % _capacity;
                _count--;
                return true;
            }

            value = default;
            return false;
        }

        public bool TryPeekFirst(out T value)
        {
            if (_count > 0)
            {
                value = _buffer[_head];
                return true;
            }

            value = default;
            return false;
        }

        public bool TryPeekLast(out T value)
        {
            if (_count > 0)
            {
                value = _buffer[_tail];
                return true;
            }

            value = default;
            return false;
        }

        public void Reverse()
        {
            var temp = _head;
            _head = _tail;
            _tail = temp;
        }

        public void TrimExcess()
        {
            TrimExcess(_count);
        }

        public void TrimExcess(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException($"capacity should be greater than 0: {capacity}", nameof(capacity));
            }

            if (capacity <= _count)
            {
                return;
            }

            Resize(capacity);
        }

        public void EnsureCapcity(int capacity)
        {
            if(capacity <= 0)
            {
                throw new ArgumentException($"capacity should be greater than 0: {capacity}", nameof(capacity));
            }

            if(capacity > _capacity)
            {
                Resize(capacity);
            }
        }

        private void Resize()
        {
            Resize(_capacity * 2);
        }

        private void Resize(int capacity)
        {
            if (_buffer == null)
                return;

            T* newBuffer = GetAllocator()!.Allocate<T>(capacity);

            int i = _tail;
            int j = 0;

            while(i != _head)
            {
                newBuffer[j++] = _buffer[i];
                i = (i + 1) % capacity;
            }

            GetAllocator()!.Free(_buffer);
            _buffer = newBuffer;
            _capacity = capacity;
            _head = _count;
            _tail = 0;
        }

        public void Dispose()
        {
            if (_buffer == null)
                return;

            if (Allocator.IsCached(_allocatorID))
            {
                GetAllocator()!.Free(_buffer);
                _buffer = null;
                _count = 0;
                _head = 0;
                _tail = 0;
            }
        }
    }
}
