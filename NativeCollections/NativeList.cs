﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NativeCollections.Utility;

namespace NativeCollections
{
    unsafe public struct NativeList<T> : IDisposable where T : unmanaged
    {
        internal void* _buffer;
        private int _capacity;
        private int _count;

        public NativeList(int initialCapacity)
        {
            if (initialCapacity <= 0)
                throw new ArgumentException($"initialCapacity should be greater than 0: {initialCapacity}");

            _buffer = Allocator.Alloc(sizeof(T) * initialCapacity);
            _capacity = initialCapacity;
            _count = 0;
        }

        public NativeList(Span<T> elements)
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

                void* source = Unsafe.AsPointer(ref MemoryMarshal.GetReference(elements));
                Unsafe.CopyBlock(_buffer, source, (uint)(sizeof(T) * _capacity));
            }
        }

        public int Length => _count;

        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value < _count || value <= 0)
                    throw new ArgumentException("Capacity", $"{value}");

                SetCapacity(value);
            }
        }

        public bool IsValid => _buffer != null;

        public bool IsEmpty => _count == 0;

        public ref T this[int index]
        {
            get
            {
                if (index < 0 || index > _count)
                    throw new ArgumentOutOfRangeException("index", $"{index}");

                ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
                return ref Unsafe.Add(ref startAddress, index);
            }
        }

        public void Add(T value)
        {
            if (_count == _capacity)
            {
                EnsureCapacity(_count + 1);
            }

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            Unsafe.Add(ref startAddress, _count++) = value;
        }

        public void AddRange(Span<T> span)
        {
            if (span.IsEmpty)
                return;

            void* startAddress = Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
            AddRange(startAddress, span.Length);
        }

        public void AddRange(NativeArray<T> array)
        {
            if (array.Length == 0)
                return;

            AddRange(array._buffer, array.Length);
        }

        private void AddRange(void* startAddress, int length)
        {
            if (startAddress == null)
                throw new ArgumentException("Invalid startAddress");

            EnsureCapacity(_count + length);

            void* destination = ((byte*)_buffer) + sizeof(T) * _count;
            Unsafe.CopyBlock(destination, startAddress, (uint)(length * sizeof(T)));
            _count += length;
        }

        public bool Remove(T value)
        {
            int index = IndexOf(value);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        public int RemoveRange(int start, int end)
        {
            if (start < 0 || end < 0 || start > _count || end > _count || start > end)
                throw new ArgumentOutOfRangeException($"Invalid range: {start} - {end}");

            if(start == 0 && end == _count)
            {
                int count = _count;
                Clear();
                return count;
            }

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            int length = end - start;

            for (int i = end; i < _count; i++)
            {
                Unsafe.Add(ref startAddress, start + i) = Unsafe.Add(ref startAddress, i);
            }

            _count -= length;
            return end - start;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index > _count)
                throw new ArgumentOutOfRangeException("index", $"{index}");

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            int length = _count - index - 1;

            for(int i = 0; i < length; i++)
            {
                Unsafe.Add(ref startAddress, i) = Unsafe.Add(ref startAddress, i + 1);
            }

            Unsafe.Add(ref startAddress, _count) = default;
            _count--;
        }

        public int ReplaceAll(T value, T newValue)
        {
            return ReplaceAll(value, newValue, 0, _count - 1);
        }

        public int ReplaceAll(T value, T newValue, int start)
        {
            return ReplaceAll(value, newValue, start, _count - 1);
        }

        public int ReplaceAll(T value, T newValue, int start, int end)
        {
            return NativeCollectionUtils.ReplaceAll(_buffer, start, end, value, newValue);
        }

        public void Clear()
        {
            if (_count == 0)
                return;

            ref byte startAddress = ref Unsafe.As<T, byte>(ref Unsafe.AsRef<T>(_buffer));
            uint length = (uint)(sizeof(T) * _count);
            Unsafe.InitBlockUnaligned(ref startAddress, 0, length);
            _count = 0;
        }

        public void Reverse()
        {
            Reverse(0, _capacity - 1);
        }

        public void Reverse(int start)
        {
            Reverse(start, _capacity - 1);
        }

        public void Reverse(int start, int end)
        {
            NativeCollectionUtils.Reverse<T>(_buffer, start, end);
        }

        public int IndexOf(T value)
        {
            return IndexOf(value, 0, _count - 1);
        }

        public int IndexOf(T value, int start)
        {
            return IndexOf(value, start, _count - 1);
        }

        public int IndexOf(T value, int start, int end)
        {
            if (_count == 0)
                return -1;

            return NativeCollectionUtils.IndexOf(_buffer, start, end, value);
        }

        public int LastIndexOf(T value)
        {
            return LastIndexOf(value, 0, _count - 1);
        }

        public int LastIndexOf(T value, int start)
        {
            return LastIndexOf(value, start, _count - 1);
        }

        public int LastIndexOf(T value, int start, int end)
        {
            if (_count == 0)
                return -1;

            return NativeCollectionUtils.LastIndexOf(_buffer, start, end, value);
        }

        public bool Contains(T value)
        {
            return IndexOf(value) >= 0;
        }

        public void TrimExcess()
        {
            if(_count != _capacity && _capacity > 4)
            {
                SetCapacity(_count);
            }
        }

        private void EnsureCapacity(int min)
        {
            if(min > _capacity)
            {
                if(min < _capacity * 2)
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
            Allocator.ReAlloc(_buffer, sizeof(T) * newCapacity);
            _capacity = newCapacity;
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