using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NativeCollections.Memory;
using NativeCollections.Utility;

namespace NativeCollections
{
    unsafe public struct NativeArray<T> : IDisposable where T : unmanaged
    {
        internal void* _buffer;
        private int _capacity;

        public NativeArray(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException($"capacity must be greater than 0: {capacity}");

            _buffer = Allocator.Default.Allocate(sizeof(T) * capacity);
            _capacity = capacity;
        }

        public NativeArray(Span<int> elements)
        {
            if (elements.IsEmpty)
            {
                this = default;
            }
            else
            {
                _buffer = Allocator.Default.Allocate(sizeof(T) * elements.Length);
                _capacity = elements.Length;

                void* source = Unsafe.AsPointer(ref MemoryMarshal.GetReference(elements));
                Unsafe.CopyBlock(_buffer, source, (uint)(sizeof(T) * _capacity));
            }
        }

        public int Length => _capacity;

        public bool IsValid => _buffer != null;

        public ref T this[int index]
        {
            get
            {
                if (index < 0 || index > _capacity)
                    throw new ArgumentOutOfRangeException("index", $"{index}");

                ref T pointer = ref Unsafe.AsRef<T>(_buffer);
                return ref Unsafe.Add(ref pointer, index);
            }
        }

        public ref T this[Index index]
        {
            get
            {
                int i = index.IsFromEnd ? _capacity - index.Value - 1: index.Value;

                if (i < 0 || i > _capacity)
                    throw new ArgumentOutOfRangeException("index", $"{index}");

                ref T pointer = ref Unsafe.AsRef<T>(_buffer);
                return ref Unsafe.Add(ref pointer, i);
            }
        }

        public NativeSlice<T> this[Range range]
        {
            get
            {
                Index startIndex = range.Start;
                Index endIndex = range.End;

                int start = startIndex.IsFromEnd ? _capacity - startIndex.Value - 1 : startIndex.Value;
                int end = endIndex.IsFromEnd ? _capacity - endIndex.Value - 1: endIndex.Value;

                if (start < 0 || end < 0 || start > _capacity || end > _capacity || start > end)
                    throw new ArgumentOutOfRangeException("range", range.ToString());

                ref T pointer = ref Unsafe.Add(ref Unsafe.AsRef<T>(_buffer), start);
                int length = end - start + 1;
                return new NativeSlice<T>(ref pointer, length);
            }
        }

        public void Fill(T value)
        {
            if (_capacity == 0)
                return;

            //Copied from Span.Fill
            if(sizeof(T) == 1)
            {
                ref byte pointer = ref Unsafe.As<T, byte>(ref Unsafe.AsRef<T>(_buffer));
                Unsafe.InitBlockUnaligned(ref pointer, Unsafe.As<T, byte>(ref value), (uint)_capacity);
            }
            else
            {
                ulong num = (uint)_capacity;
                if (num != 0L)
                {
                    ref T value2 = ref Unsafe.AsRef<T>(_buffer);
                    ulong size = (uint)Unsafe.SizeOf<T>();
                    ulong pos;
                    for (pos = 0uL; pos < (ulong)((long)num & -8L); pos += 8)
                    {
                        Unsafe.AddByteOffset(ref value2, (IntPtr)(pos * size)) = value;
                        Unsafe.AddByteOffset(ref value2, (IntPtr)((pos + 1) * size)) = value;
                        Unsafe.AddByteOffset(ref value2, (IntPtr)((pos + 2) * size)) = value;
                        Unsafe.AddByteOffset(ref value2, (IntPtr)((pos + 3) * size)) = value;
                        Unsafe.AddByteOffset(ref value2, (IntPtr)((pos + 4) * size)) = value;
                        Unsafe.AddByteOffset(ref value2, (IntPtr)((pos + 5) * size)) = value;
                        Unsafe.AddByteOffset(ref value2, (IntPtr)((pos + 6) * size)) = value;
                        Unsafe.AddByteOffset(ref value2, (IntPtr)((pos + 7) * size)) = value;
                    }
                    if (pos < (ulong)((long)num & -4L))
                    {
                        Unsafe.AddByteOffset(ref value2, (IntPtr)(pos * size)) = value;
                        Unsafe.AddByteOffset(ref value2, (IntPtr)((pos + 1) * size)) = value;
                        Unsafe.AddByteOffset(ref value2, (IntPtr)((pos + 2) * size)) = value;
                        Unsafe.AddByteOffset(ref value2, (IntPtr)((pos + 3) * size)) = value;
                        pos += 4;
                    }
                    for (; pos < num; pos++)
                    {
                        Unsafe.AddByteOffset(ref value2, (IntPtr)(pos * size)) = value;
                    }
                }
            }
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
            return IndexOf(value, 0, _capacity - 1);
        }

        public int IndexOf(T value, int start)
        {
            return IndexOf(value, start, _capacity - 1);
        }

        public int IndexOf(T value, int start, int end)
        {
            return NativeCollectionUtils.IndexOf(_buffer, start, end, value);
        }

        public int LastIndexOf(T value)
        {
            return LastIndexOf(value, 0, _capacity - 1);
        }

        public int LastIndexOf(T value, int start)
        {
            return LastIndexOf(value, start, _capacity - 1);
        }

        public int LastIndexOf(T value, int start, int end)
        {
            return NativeCollectionUtils.LastIndexOf(_buffer, start, end, value);
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
