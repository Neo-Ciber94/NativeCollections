using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NativeCollections.Allocators;
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

            _buffer = Allocator.Default.Allocate(Unsafe.SizeOf<T>() * initialCapacity);
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
                _buffer = Allocator.Default.Allocate(Unsafe.SizeOf<T>() * elements.Length);
                _capacity = elements.Length;
                _count = _capacity;

                void* source = Unsafe.AsPointer(ref MemoryMarshal.GetReference(elements));
                Unsafe.CopyBlock(_buffer, source, (uint)(Unsafe.SizeOf<T>() * _capacity));
            }
        }

        public NativeList(void* pointer, int length)
        {
            if (pointer == null)
                throw new ArgumentException("Invalid pointer");

            if (length <= 0)
                throw new ArgumentException($"Invalid length: {length}", nameof(length));

            _buffer = pointer;
            _capacity = length;
            _count = length;
        }

        public int Length => _count;

        public int Capacity
        {
            get => _capacity;
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
                ResizeIfNeeded(_count + 1);
            }

            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            Unsafe.Add(ref startAddress, _count++) = value;
        }

        public void AddRange(Span<T> span)
        {
            if (span.IsEmpty)
                throw new ArgumentException("Empty span");

            void* startAddress = Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
            AddRange(startAddress, span.Length);
        }

        public void AddRange(NativeArray<T> array)
        {
            if (array.IsValid)
                throw new ArgumentException("Invalid array");

            AddRange(array._buffer, array.Length);
        }

        private void AddRange(void* source, int length)
        {
            if (source == null)
                throw new ArgumentException("Invalid startAddress");

            if (length <= 0)
                throw new ArgumentException(nameof(length), length.ToString());

            ResizeIfNeeded(_count + length);

            void* destination = ((byte*)_buffer) + Unsafe.SizeOf<T>() * _count;
            Unsafe.CopyBlock(destination, source, (uint)(length * Unsafe.SizeOf<T>()));
            _count += length;
        }

        public void Insert(int index, T value)
        {
            InsertRange(index, Unsafe.AsPointer(ref value), 1);
        }

        public void InsertRange(int index, Span<T> span)
        {
            if (span.IsEmpty)
                throw new ArgumentException("Empty span");

            void* pointer = Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
            InsertRange(index, pointer, span.Length);
        }

        public void InsertRange(int index, NativeArray<T> array)
        {
            if (array.IsValid)
                throw new ArgumentException("Invalid array");
           
            InsertRange(index, array._buffer, array.Length);
        }

        private void InsertRange(int index, void* source, int length)
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException("index", index.ToString());

            if (source == null)
                throw new ArgumentException("Invalid pointer");

            if (length <= 0)
                throw new ArgumentException(nameof(length), length.ToString());

            ResizeIfNeeded(_count + length);

            int size = _count - index;
            ref T startAddress = ref Unsafe.AsRef<T>(_buffer);
            ref T insertionIndex = ref Unsafe.Add(ref startAddress, index);

            void* src = Unsafe.AsPointer(ref insertionIndex);
            void* dest = Unsafe.AsPointer(ref Unsafe.Add(ref insertionIndex, index + length));
            Unsafe.CopyBlock(dest, src, (uint)(Unsafe.SizeOf<T>() * size));
            Unsafe.CopyBlock(dest, source, (uint)length);
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

            if (start == 0 && end == _count)
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

            for (int i = 0; i < length; i++)
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
            return NativeCollectionUtility.ReplaceAll(_buffer, start, end, value, newValue);
        }

        public void Clear()
        {
            if (_count == 0)
                return;

            ref byte startAddress = ref Unsafe.As<T, byte>(ref Unsafe.AsRef<T>(_buffer));
            uint length = (uint)(Unsafe.SizeOf<T>() * _count);
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
            NativeCollectionUtility.Reverse<T>(_buffer, start, end);
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

            return NativeCollectionUtility.IndexOf(_buffer, start, end, value);
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

            return NativeCollectionUtility.LastIndexOf(_buffer, start, end, value);
        }

        public bool Contains(T value)
        {
            return IndexOf(value) >= 0;
        }

        public void TrimExcess()
        {
            TrimExcess(_count);
        }

        public void TrimExcess(int capacity)
        {
            if (capacity <= _count)
            {
                return;
            }

            SetCapacity(capacity);
        }

        public void EnsureCapacity(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException(capacity.ToString(), nameof(capacity));
            }

            if(capacity > _capacity)
            {
                SetCapacity(capacity);
            }
        }

        public void* GetUnsafePointer()
        {
            return _buffer;
        }

        private void ResizeIfNeeded(int min)
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
            Allocator.Default.ReAllocate(_buffer, Unsafe.SizeOf<T>() * newCapacity);
            _capacity = newCapacity;
        }

        public void Dispose()
        {
            if (_buffer == null)
                return;

            Allocator.Default.Free(_buffer);
            _buffer = null;
            _capacity = 0;
            _count = 0;
        }
    }
}
