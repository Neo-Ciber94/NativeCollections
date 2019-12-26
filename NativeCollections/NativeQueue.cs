﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NativeCollections.Utility;

namespace NativeCollections
{
    unsafe public struct NativeQueue<T> : IDisposable where T: unmanaged
    {
        private void* _buffer;
        private int _capacity;
        private int _count;

        private int _head;
        private int _tail;

        public NativeQueue(int initialCapacity)
        {
            if (initialCapacity <= 0)
                throw new ArgumentException($"capacity must be greater than 0: {initialCapacity}");

            _buffer = Allocator.Alloc(sizeof(T) * initialCapacity);
            _capacity = initialCapacity;
            _count = _head = _tail = 0;
        }

        public NativeQueue(Span<int> elements)
        {
            if (elements.IsEmpty)
            {
                this = default;
            }
            else
            {
                _buffer = Allocator.Alloc(sizeof(T) * elements.Length);
                _capacity = elements.Length;
                _count = _capacity;
                _head = _tail = 0;

                void* source = Unsafe.AsPointer(ref MemoryMarshal.GetReference(elements));
                Unsafe.CopyBlock(_buffer, source, (uint)(sizeof(T) * _capacity));
            }
        }

        public int Length => _count;

        public int Capacity => _capacity;

        public bool IsValid => _buffer != null;

        public bool IsEmpty => _count == 0;

        public void Enqueue(T value)
        {
            if(_count == _capacity || _head == _tail)
            {
                EnsureCapacity(_count + 1);
            }

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            Unsafe.Add(ref startAddress, _tail) = value;
            _tail = (_tail + 1) % _capacity;
            _count++;
        }

        public T Dequeue()
        {
            if (_count == 0)
                throw new InvalidOperationException("The Queue is empty");

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            ref T value = ref Unsafe.Add(ref startAddress, _head);
            Unsafe.Add(ref startAddress, _head) = default;
            _head = (_head + 1) % _capacity;
            _count--;
            return value;
        }

        public T Peek()
        {
            if (_count == 0)
                throw new InvalidOperationException("The Queue is empty");

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            ref T value = ref Unsafe.Add(ref startAddress, _head);
            Unsafe.Add(ref startAddress, _head) = default;
            return value;
        }

        public bool TryDequeue(out T value)
        {
            if(_count == 0)
            {
                value = default;
                return false;
            }
            else
            {
                ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
                value = Unsafe.Add(ref startAddress, _head);
                Unsafe.Add(ref startAddress, _head) = default;
                _head = (_head + 1) % _capacity;
                _count--;
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
                value = Unsafe.Add(ref startAddress, _head);
                Unsafe.Add(ref startAddress, _head) = default;
                return true;
            }
        }

        public bool Contains(T value)
        {
            return NativeCollectionUtils.IndexOf<T>(_buffer, _capacity, value) >= 0;
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

            void* newBuffer = Allocator.Alloc(sizeof(T) * newCapacity);
            ref T destination = ref Unsafe.AsRef<T>(newBuffer);
            ref T source = ref Unsafe.AsRef<T>(_buffer);
            int head = (_head + 1) % _capacity;

            int i = 0;
            while (head != _tail)
            {
                Unsafe.Add(ref destination, i++) = Unsafe.Add(ref source, head);
                head = (_head + 1) % _capacity;
            }

            Allocator.Free(_buffer);
            _buffer = newBuffer;
            _capacity = newCapacity;
            _tail = _count;
            _head = 0;
        }

        public void Dispose()
        {
            if (_buffer == null)
                return;

            Allocator.Free(_buffer);
            _buffer = null;
            _capacity = 0;
            _count = 0;
        }
    }
}