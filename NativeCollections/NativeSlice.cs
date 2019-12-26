using System;
using System.Runtime.CompilerServices;
using NativeCollections.Utility;

namespace NativeCollections
{
    unsafe public readonly ref struct NativeSlice<T>
    {
        private readonly void* _pointer;
        private readonly int _length;

        public NativeSlice(void* pointer, int length)
        {
            if (pointer == null)
                throw new ArgumentException("Invalid pointer");

            if (length < 0)
                throw new ArgumentException($"Invalid length: {length}");

            _pointer = pointer;
            _length = length;
        }

        public NativeSlice(ref T pointer, int length)
        {
            if (pointer == null)
                throw new ArgumentException("Invalid pointer");

            if (length < 0)
                throw new ArgumentException($"Invalid length: {length}");

            _pointer = Unsafe.AsPointer(ref pointer);
            _length = length;
        }

        public int Length => _length;

        public ref T this[int index]
        {
            get
            {
                if (index < 0 || index > _length)
                    throw new ArgumentOutOfRangeException("index", $"{index}");

                ref T pointer = ref Unsafe.AsRef<T>(_pointer);
                return ref Unsafe.Add(ref pointer, index);
            }
        }

        public ref T this[Index index]
        {
            get
            {
                int i = index.IsFromEnd ? _length - index.Value - 1 : index.Value;

                if (i < 0 || i > _length)
                    throw new ArgumentOutOfRangeException("index", $"{index}");

                ref T pointer = ref Unsafe.AsRef<T>(_pointer);
                return ref Unsafe.Add(ref pointer, i);
            }
        }

        public NativeSlice<T> this[Range range]
        {
            get
            {
                Index startIndex = range.Start;
                Index endIndex = range.End;

                int start = startIndex.IsFromEnd ? _length - startIndex.Value : startIndex.Value;
                int end = endIndex.IsFromEnd ? _length - endIndex.Value : endIndex.Value;

                if (start < 0 || end < 0 || start > _length || end > _length || start > end)
                    throw new ArgumentOutOfRangeException("range", range.ToString());

                ref T pointer = ref Unsafe.Add(ref Unsafe.AsRef<T>(_pointer), start);
                int length = end - start + 1;
                return new NativeSlice<T>(ref pointer, length);
            }
        }

        public Span<T> Span
        {
            get => new Span<T>(_pointer, _length);
        }

        public RefEnumerator<T> GetEnumerator() => new RefEnumerator<T>(_pointer, _length);
    }
}
